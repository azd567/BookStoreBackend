using BookStoreBackend.DTOs;
using BookStoreBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookStoreBackend.Controllers
{
    [RoutePrefix("api/search")]
    public class SearchController : ApiController
    {
        
        private bookstoreDBEntities db = new bookstoreDBEntities();

        // GET: api/Search/Category
        [Route("category")]
        [HttpGet]
        public IQueryable<BookDTO> GetBooksByCategory(string name)
        {
            return BookDTO.SerializeBookList(db.Books.Where(book => book.Category.Name.Contains(name) && book.Status));      
        }

        // GET: api/Search/Author
        [Route("author")]
        [HttpGet]
        public IQueryable<BookDTO> GetBooksByAuthor(string name)
        {
            return BookDTO.SerializeBookList(db.Books.Where(book => book.AuthorName.Contains(name) && book.Status));
        }

        // GET: api/Search/Title
        [Route("title")]
        [HttpGet]
        public IQueryable<BookDTO> GetBooksByTitle(string name)
        {
            return BookDTO.SerializeBookList(db.Books.Where(book => book.Title.Contains(name) && book.Status));
        }

        // GET: api/Search/ISBN
        [Route("isbn")]
        [HttpGet]
        public IQueryable<BookDTO> GetBooksByISBN(string number)
        {
            return BookDTO.SerializeBookList(db.Books.Where(book => book.ISBN.Contains(number) && book.Status));
        }

        // GET: api/Search/User
        [Route("user")]
        [HttpGet]
        public IQueryable<UserDTO> GetUsersByName(string name)
        {
            return UserDTO.SerializeUserList(db.AppUsers.Where(user => user.UserName.Contains(name) && user.IsAdmin == false));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
