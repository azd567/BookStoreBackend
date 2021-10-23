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
using BookStoreBackend.Models;

namespace BookStoreBackend.Controllers
{
    public class BooksController : ApiController
    {
        private bookstoreDBEntities db = new bookstoreDBEntities();

        // GET: api/Books
        public IQueryable<BookDTO> GetBooks()
        {
            return BookDTO.SerializeBookList(db.Books.Where(book => book.Status));
        }

        // GET: api/Books/5
        [ResponseType(typeof(BookDTO))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(new BookDTO(book));
        }

        // PUT: api/Books
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBook(BookDTO bk)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (bk.BookId == null)
            {
                return BadRequest();
            }

            if (!BookExists(bk.BookId.Value))
            {
                return NotFound();
            }

            Book book = await db.Books.FindAsync(bk.BookId.Value);

            book.Title = bk.Title ?? book.Title;
            book.Featured = bk.Featured ?? book.Featured;
            book.Description = bk.Description ?? book.Description;
            book.Image = bk.Image ?? book.Image;
            book.Status = bk.Status ?? book.Status;
            book.Qty = bk.Qty ?? book.Qty;
            book.Price = bk.Price ?? book.Price;
            book.Position = bk.Position ?? book.Position;


            db.Entry(book).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Books
        public async Task<IHttpActionResult> PostBook(BookDTO bk)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Book book = new Book
            {
                Title = bk.Title,
                ISBN = bk.ISBN,
                AuthorName = bk.Author,
                CategoryId = bk.CategoryId.Value,
                Image = bk.Image,
                Status = bk.Status.Value,
                Description = bk.Description,
                Price = bk.Price.Value,
                Position = bk.Position.Value,
                Featured = bk.Featured.Value,
                Year = bk.Year.Value,
                Qty = bk.Qty.Value
            };


            try
            {
                db.Books.Add(book);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return CreatedAtRoute("DefaultApi", new { id = book.BookId }, book.BookId);
        }

        // DELETE: api/Books/5
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            await db.SaveChangesAsync();

            return Ok(book);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.BookId == id) > 0;
        }
    }
}