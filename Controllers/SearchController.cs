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
        public IEnumerable<BookDTO> GetBooksByCategory(string data)
        {
            return BookDTO.SerializeBookList(db.Books.Where(book => book.Category.Name.Contains(data) && book.Status));      
        }

        // GET: api/Search/Author
        [Route("author")]
        [HttpGet]
        public IEnumerable<BookDTO> GetBooksByAuthor(string data)
        {
            return BookDTO.SerializeBookList(db.Books.Where(book => book.AuthorName.Contains(data) && book.Status));
        }

        // GET: api/Search/Title
        [Route("title")]
        [HttpGet]
        public IEnumerable<BookDTO> GetBooksByTitle(string data)
        {
            return BookDTO.SerializeBookList(db.Books.Where(book => book.Title.Contains(data) && book.Status));
        }

        // GET: api/Search/ISBN
        [Route("isbn")]
        [HttpGet]
        public IEnumerable<BookDTO> GetBooksByISBN(string data)
        {
            return BookDTO.SerializeBookList(db.Books.Where(book => book.ISBN.Contains(data) && book.Status));
        }

        // GET: api/Search/User
        [Route("user")]
        [HttpGet]
        [Authorize]
        public IQueryable<UserDTO> GetUsersByName(string data)
        {
            return UserDTO.SerializeUserList(db.AppUsers.Where(user => user.UserName.Contains(data) && user.IsAdmin == false));
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
