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
using BookStoreBackend.Models;
using BookStoreBackend.DTOs;

namespace BookStoreBackend.Controllers
{
    [RoutePrefix("api/appusers")]
    public class AppUsersController : ApiController
    {
        private bookstoreDBEntities db = new bookstoreDBEntities();

        // GET: api/AppUsers
        public IQueryable<UserDTO> GetAppUsers()
        {
            return db.AppUsers.Select(
                User => new UserDTO { 
                Id=User.UserId, 
                Name = User.UserName, 
                Address = User.UserAddress,
                IsActive=User.IsActive, 
                IsAdmin= User.IsAdmin });
        }

        // GET: api/AppUsers/5
        [ResponseType(typeof(UserDTO))]
        public async Task<IHttpActionResult> GetAppUser(int id)
        {
            AppUser appUser = await db.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            return Ok(new UserDTO(appUser));
        }

        // PUT: api/AppUsers
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAppUser(UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user.Id == null)
            {
                return BadRequest();
            }

            if (!AppUserExists(user.Id.Value))
            {
                return NotFound();
            }

            AppUser appUser = await db.AppUsers.FindAsync(user.Id.Value);

            appUser.IsAdmin = user.IsAdmin ?? appUser.IsAdmin;
            appUser.UserName = user.Name ?? appUser.UserName;
            appUser.UserAddress = user.Address ?? appUser.UserAddress;
            appUser.IsActive = user.IsActive ?? appUser.IsActive;

            db.Entry(appUser).State = EntityState.Modified;

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

        // POST: api/AppUsers
        [ResponseType(typeof(UserDTO))]
        public async Task<IHttpActionResult> PostAppUser(UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            AppUser appUser = new AppUser
            {
                UserName = user.Name,
                UserAddress = user.Address,
                UserPassword = System.Web.Helpers.Crypto.SHA1(user.Password),
                IsActive = user.IsActive.Value,
                IsAdmin = user.IsAdmin.Value
            };

            try
            {
                db.AppUsers.Add(appUser);
                await db.SaveChangesAsync();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }

            return CreatedAtRoute("DefaultApi", new { id = appUser.UserId }, user);
        }

        // DELETE: api/AppUsers/5
        [ResponseType(typeof(AppUser))]
        public async Task<IHttpActionResult> DeleteAppUser(int id)
        {
            AppUser appUser = await db.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            db.AppUsers.Remove(appUser);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(appUser);
        }

        // GET: api/AppUsers/WishList?id={id}
        [Route("wishlist")]
        [HttpGet]
        public async Task<IQueryable<BookDTO>> GetWishList(int id)
        {
            AppUser appUser = await db.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return null;
            }

            return appUser.Books.Select(book => new BookDTO(book)).AsQueryable(); ;
        }

        // PUT: api/AppUsers/WishList?UserId={UserId}&BookId={BookId}
        [Route("wishlist")]
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddNewToWishList(int UserId, int BookId)
        {

            AppUser appUser = await db.AppUsers.FindAsync(UserId);

            Book book = await db.Books.FindAsync(BookId);

            if (appUser == null || book == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(appUser.Books.Contains(book))
            {
                return Conflict();
            }

            appUser.Books.Add(book);

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

        // DELETE: api/AppUsers/WishList?UserId={UserId}&BookId={BookId}
        [Route("wishlist")]
        [HttpDelete]
        public async Task<IHttpActionResult> RemoveFromWishList(int UserId, int BookId)
        {

            AppUser appUser = await db.AppUsers.FindAsync(UserId);

            Book book = await db.Books.FindAsync(BookId);

            if (appUser == null || book == null || !appUser.Books.Contains(book))
            {
                return NotFound();
            }

            appUser.Books.Remove(book);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AppUserExists(int id)
        {
            return db.AppUsers.Count(e => e.UserId == id) > 0;
        }
    }
}