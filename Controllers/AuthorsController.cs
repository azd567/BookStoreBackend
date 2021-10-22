using System;
using System.Collections.Generic;
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
    public class AuthorsController : ApiController
    {
        private bookstoreDBEntities db = new bookstoreDBEntities();

        public IQueryable<AuthorsDTO> GetAuthors() {
            var Authors = from x in db.Authors
                          select new AuthorsDTO
                          {
                              AuthorId = x.AuthorId,
                              AuthorName = x.AuthorName

                          };
            return Authors;
        }

        [ResponseType(typeof(AuthorsDTO))]
        public IHttpActionResult GetAuthors(int id)
        {
            var author = db.Authors.Select(
                authors => new AuthorsDTO()
                {
                    AuthorId = authors.AuthorId,
                    AuthorName = authors.AuthorName
                }).SingleOrDefault(b => b.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            return Ok(author);
        }

        [ResponseType(typeof(void))]
        public IHttpActionResult PutAuthor(int id, Author author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != author.AuthorId)
            {
                return BadRequest();
            }

            db.Entry(author).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
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

        // POST: api/Authors
        [ResponseType(typeof(Author))]
        public IHttpActionResult PostAuthor(Author author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Authors.Add(author);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AuthorExists(author.AuthorId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = author.AuthorId }, author);
        }

        // DELETE: api/Authors/5
        [ResponseType(typeof(Author))]
        public IHttpActionResult DeleteAuthor(int id)
        {
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return NotFound();
            }

            db.Authors.Remove(author);
            db.SaveChanges();

            return Ok(author);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AuthorExists(int id)
        {
            return db.Authors.Count(e => e.AuthorId == id) > 0;
        }
    }
}

  
