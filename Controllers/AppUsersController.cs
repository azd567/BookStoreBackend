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
using BookStoreBackend.Interfaces;
using System.Web.Helpers;
using NLog;
using System.Web.Http.Cors;

namespace BookStoreBackend.Controllers
{
    [RoutePrefix("api/appusers")]
    public class AppUsersController : ApiController
    {
        private bookstoreDBEntities db = new bookstoreDBEntities();

        private readonly ILoggerService _logger;

        private readonly ITokenService _tokenService;

        public AppUsersController(ITokenService tokenService, ILoggerService logger)
        {
            this._tokenService = tokenService;
            this._logger = logger;
        }

        // GET: api/AppUsers
        public IQueryable<UserDTO> GetAppUsers()
        {
            return UserDTO.SerializeUserList(db.AppUsers);
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

        // PUT: api/AppUsers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAppUser(int id, UserDTO user)
        {

            this._logger.Info("Started HTTP PUT Request for updating User details");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user == null || user.Id == null)
            {
                return BadRequest("No User Id provided");
            }

            if (id != user.Id.Value)
            {
                return BadRequest("URI Dosen't match with perovided id");
            }

            if (!AppUserExists(user.Id.Value))
            {
                return NotFound();
            }

            try
            {
                AppUser appUser = await db.AppUsers.FindAsync(user.Id.Value);

                appUser.IsAdmin = user.IsAdmin ?? appUser.IsAdmin;
                appUser.UserName = user.Name ?? appUser.UserName;
                appUser.UserAddress = user.Address ?? appUser.UserAddress;
                appUser.IsActive = user.IsActive ?? appUser.IsActive;

                db.Entry(appUser).State = EntityState.Modified;
            
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.Error("Exception in AppUserController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success in HTTP PUT Request for updating User details");

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST : api/AppUsers/Login
        [Route("login", Name = "LoginAppUserApi")]
        public IHttpActionResult LoginAppUser(UserDTO user)
        {
            this._logger.Info("Started HTTP POST Request for User login");

            if (user == null || user.Name == null)
                return BadRequest("No User name provided");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = db.AppUsers.Where(appuser => appuser.UserName == user.Name).FirstOrDefault();

            if (appUser == null)
                return BadRequest("No User exists with the provided name");

            try
            {
                if (!appUser.IsActive)
                    return BadRequest("User account is deactivated by Admin");

                if (!Crypto.VerifyHashedPassword(appUser.UserPassword, user.Password))
                    return BadRequest("Invalid Credentials"); 
            }
            catch (Exception e)
            {
                this._logger.Error("Exception in AppUserController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success in HTTP POST Request for User login");

            return CreatedAtRoute("LoginAppUserApi", new { id = appUser.UserId }, this._tokenService.CreateToken(appUser.UserId, appUser.UserName, appUser.IsAdmin));
        }

        // POST: api/AppUsers/Register
        [Route("register", Name = "RegisterAppUserApi")]
        public async Task<IHttpActionResult> RegisterAppUser(UserDTO user)
        {
            this._logger.Info("Started HTTP POST Request for Registering User");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (user == null || user.Name == null || user.Password == null)
                return BadRequest("All required fields are not provided");

            if ((await db.AppUsers.AnyAsync(appuser => appuser.UserName == user.Name)))
                return BadRequest("Given User name already exists");

            AppUser appUser = new AppUser();

            try
            {
                appUser.UserName = user.Name;
                appUser.UserAddress = user.Address;
                appUser.UserPassword = Crypto.HashPassword(user.Password);
                appUser.IsActive = user.IsActive == null || user.IsActive.Value;
                appUser.IsAdmin = user.IsAdmin != null && user.IsAdmin.Value;

                db.AppUsers.Add(appUser);
                await db.SaveChangesAsync();
            }
            catch(Exception e)
            {
                this._logger.Error("Exception in AppUserController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success in HTTP POST Request for Registering User");

            return CreatedAtRoute("RegisterAppUserApi", new { id = appUser.UserId }, this._tokenService.CreateToken(appUser.UserId, appUser.UserName, appUser.IsAdmin));
        }

        // DELETE: api/AppUsers/5
        [ResponseType(typeof(AppUser))]
        public async Task<IHttpActionResult> DeleteAppUser(int id)
        {
            this._logger.Info("Started HTTP DELETE Request for Deleting User");

            AppUser appUser = await db.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            try
            {
                db.AppUsers.Remove(appUser);

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.Error("Exception in AppUserController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success in HTTP DELETE Request for Deleting User");

            return Ok(appUser);
        }

        // GET: api/AppUsers/WishList?id={id}
        [Route("wishlist")]
        [HttpGet]
        public async Task<IHttpActionResult> GetWishList(int id)
        {
            AppUser appUser = await db.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            return Ok(appUser.Books.Select(book => new BookDTO(book)).AsQueryable());
        }

        // PUT: api/AppUsers/WishList?UserId={UserId}&BookId={BookId}
        [Route("wishlist")]
        [HttpPut]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> AddNewToWishList(int UserId, int BookId)
        {
            this._logger.Info("Started HTTP PUT Request for Updating User wishlist");

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

            try
            {
                appUser.Books.Add(book);
            
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.Error("Exception in AppUserController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success in HTTP PUT Request for Updating User wishlist");

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/AppUsers/WishList?UserId={UserId}&BookId={BookId}
        [Route("wishlist")]
        [HttpDelete]
        public async Task<IHttpActionResult> RemoveFromWishList(int UserId, int BookId)
        {
            this._logger.Info("Started HTTP DELETE Request for Deleting a book entry in User wishlist");

            AppUser appUser = await db.AppUsers.FindAsync(UserId);

            Book book = await db.Books.FindAsync(BookId);

            if (appUser == null || book == null || !appUser.Books.Contains(book))
            {
                return NotFound();
            }

            try
            {
                appUser.Books.Remove(book);
            
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                this._logger.Error("Exception in AppUserController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success in HTTP DELETE Request for Deleting a book entry in User wishlist");

            return Ok();
        }


        // GET: api/AppUsers/Orders?id={id}
        [Route("orders")]
        [HttpGet]
        public async Task<IHttpActionResult> GetOrders(int id)
        {
            AppUser appUser = await db.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            return Ok(appUser.BookOrders.Select(order => new { order.OrderId, order.OrderDate }).AsQueryable());
        }

        // DELETE THIS WHEN NO LONGER REQUIRED
        // **********************************************

        //[Route("test")]
        //[HttpGet]
        //public async Task<dynamic> Gettest(int id)
        //{
        //    AppUser appUser = await db.AppUsers.FindAsync(id);
        //    if (appUser == null)
        //    {
        //        return null;
        //    }

        //    return this._tokenService.CreateToken(appUser.UserId, appUser.UserName, appUser.IsAdmin);
        //}

        //[Route("test2")]
        //[HttpGet]
        //[Authorize]
        //public string Gettest2()
        //{
        //    return "User allowed";
        //}

        //[Route("test3")]
        //[HttpGet]
        //[Authorize(Roles = "Admin")]
        //public string Gettest3()
        //{
        //    return "Admin allowed";
        //}

        // *************************************************

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