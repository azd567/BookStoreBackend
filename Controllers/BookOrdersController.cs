using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BookStoreBackend.DTOs;
using BookStoreBackend.Models;

namespace BookStoreBackend.Controllers
{
    public class BookOrdersController : ApiController
    {
        private bookstoreDBEntities db = new bookstoreDBEntities();

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
        

        // PUT: api/BookOrders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBookOrder(int id, BookOrder bookOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bookOrder.OrderId)
            {
                return BadRequest();
            }

            db.Entry(bookOrder).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/BookOrders
        [ResponseType(typeof(BookOrder))]
        public IHttpActionResult PostBookOrder(BookOrder bookOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.BookOrders.Add(bookOrder);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = bookOrder.OrderId }, bookOrder);
        }

        // DELETE: api/BookOrders/5
        [ResponseType(typeof(BookOrder))]
        public IHttpActionResult DeleteBookOrder(int id)
        {
            BookOrder bookOrder = db.BookOrders.Find(id);
            if (bookOrder == null)
            {
                return NotFound();
            }

            db.BookOrders.Remove(bookOrder);
            db.SaveChanges();

            return Ok(bookOrder);
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
