using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BookStoreBackend.DTOs;
using BookStoreBackend.Interfaces;
using BookStoreBackend.Models;

namespace BookStoreBackend.Controllers
{
    [RoutePrefix("api/bookorders")]
    public class BookOrdersController : ApiController
    {
        private bookstoreDBEntities db = new bookstoreDBEntities();

        private readonly ILoggerService _logger;

        public BookOrdersController(ILoggerService logger)
        {
            this._logger = logger;
        }

        // GET: api/BookOrders
        public IQueryable<BookOrderDTO> GetBookOrders()
        {
            return db.BookOrders.Select(
                Order => new BookOrderDTO
                {
                    OrderId = Order.OrderId,
                    OrderDate = Order.OrderDate,
                    CouponId = Order.CouponId,
                    UserId = Order.UserId
                }
                );
        }

        // GET: api/BookOrders/5
        [ResponseType(typeof(BookOrderDTO))]
        public IHttpActionResult GetBookOrder(int id)
        {
            BookOrder bookOrder = db.BookOrders.Find(id);
            if (bookOrder == null)
            {
                return NotFound();
            }
            BookOrderDTO bo = new BookOrderDTO
            {
                OrderId = bookOrder.OrderId,
                OrderDate = bookOrder.OrderDate,
                CouponId = bookOrder.CouponId,
                UserId = bookOrder.UserId
            };
            return Ok(bo);
        }


        //// PUT: api/BookOrders/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutBookOrder(int id, BookOrder bookOrder)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != bookOrder.OrderId)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(bookOrder).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BookOrderExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/BookOrders
        //[ResponseType(typeof(BookOrder))]
        //public IHttpActionResult PostBookOrder(BookOrder bookOrder)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.BookOrders.Add(bookOrder);
        //    db.SaveChanges();

        //    return CreatedAtRoute("DefaultApi", new { id = bookOrder.OrderId }, bookOrder);
        //}
        
        //// DELETE: api/BookOrders/5
        //[ResponseType(typeof(BookOrder))]
        //public IHttpActionResult DeleteBookOrder(int id)
        //{
        //    BookOrder bookOrder = db.BookOrders.Find(id);
        //    if (bookOrder == null)
        //    {
        //        return NotFound();
        //    }

        //    db.BookOrders.Remove(bookOrder);
        //    db.SaveChanges();

        //    return Ok(bookOrder);
        //}


        // POST: api/BookOrders/
        [HttpPost]
        public async Task<IHttpActionResult> PostBookOrder(BookListOrderDTO order)
        {

            this._logger.Info("Started HTTP POST Request for Placing an order");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (order.UserId == null || (await db.AppUsers.FindAsync(order.UserId.Value)).UserAddress == null)
                return BadRequest("Shipping Address is required");

            BookOrder bookOrder = new BookOrder();
            bool allBookPurchased = order.BookList.Count > 0;

            try
            {

                bookOrder.OrderDate = order.OrderDate;
                bookOrder.UserId = order.UserId.Value;
                bookOrder.CouponId = order.CouponId;

                db.BookOrders.Add(bookOrder);

                foreach (var bookQty in order.BookList)
                {
                    bool isItemValid = false;

                    if(bookQty.BookId.HasValue && bookQty.Qty.HasValue)
                    {
                        var book = await db.Books.FindAsync(bookQty.BookId.Value);

                        if (book != null && book.Qty >= bookQty.Qty.Value)
                        {
                            isItemValid = true;

                            db.OrdersBooks.Add(new OrdersBooks
                            {
                                BookId = bookQty.BookId.Value,
                                OrderId = bookOrder.OrderId,
                                Qty = bookQty.Qty.Value
                            });

                            book.Qty -= bookQty.Qty.Value;

                            db.Entry(book).State = EntityState.Modified;

                        }
                        
                    }

                    allBookPurchased &= isItemValid; 
                }

                if (allBookPurchased)
                    db.SaveChanges();
                else
                    return BadRequest("Requested list or quantity of Book(s) is invalid");
            }
            catch (Exception e)
            {
                this._logger.Error("Exception in BookOrdersController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success in HTTP POST Request for Placing an order");

            return CreatedAtRoute("DefaultApi", new { id = bookOrder.OrderId }, bookOrder.OrderId);
        }

        // GET: api/BookOrders/OrderDetails?id={id}
        [Route("orderdetails")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOrderDetails(int id)
        {
            BookOrder order = await db.BookOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(new {
                order.OrderDate,
                CouponCode = order.Coupon == null ? "null" : order.Coupon.CouponCode,
                BookList = order.OrdersBooks.Select(books => new { book = new BookDTO(books.Book), qty = books.Qty })
            }); 
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookOrderExists(int id)
        {
            return db.BookOrders.Count(e => e.OrderId == id) > 0;
        }
    }
}