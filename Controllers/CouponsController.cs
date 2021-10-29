using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BookStoreBackend.DTOs;
using BookStoreBackend.Interfaces;
using BookStoreBackend.Models;

namespace BookStoreBackend.Controllers
{
    public class CouponsController : ApiController
    {
        private bookstoreDBEntities db = new bookstoreDBEntities();
        private readonly ILoggerService _logger;

        public CouponsController(ILoggerService logger)
        {
            this._logger = logger;
        }

        // GET: api/Coupons
        public IQueryable<CouponDTO> GetCoupons()
        {
            return db.Coupons.Select(
                Coupon => new CouponDTO
                {
                    CouponId = Coupon.CouponId,
                    CouponCode = Coupon.CouponCode,
                    Discount = Coupon.Discount
                }
                );
        }

        // GET: api/Coupons/5
        [ResponseType(typeof(CouponDTO))]
        public IHttpActionResult GetCoupon(int id)
        {
            Coupon coupon = db.Coupons.Find(id);
           
            if (coupon == null)
            {
                return NotFound();
            }
            CouponDTO cp = new CouponDTO
            {
                CouponId = coupon.CouponId,
                CouponCode = coupon.CouponCode,
                Discount = coupon.Discount
            };

            return Ok(cp);
        }

        // GET: api/Coupons?Code={code}
        [ResponseType(typeof(CouponDTO))]
        public IHttpActionResult GetCouponFromCode(string code)
        {
            Coupon coupon = db.Coupons.Where(c => c.CouponCode == code).FirstOrDefault();
    
            if (coupon == null)
            {
                return NotFound();
            }
            CouponDTO cp = new CouponDTO
            {
                CouponId = coupon.CouponId,
                CouponCode = coupon.CouponCode,
                Discount = coupon.Discount
            };

            return Ok(cp);
        }

        // PUT: api/Coupons/5
        [ResponseType(typeof(void))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PutCoupon(int id, Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != coupon.CouponId)
            {
                return BadRequest();
            }

            db.Entry(coupon).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CouponExists(id))
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

        // POST: api/Coupons
        [ResponseType(typeof(Coupon))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PostCoupon(CouponDTO coup)
        {
            this._logger.Info("Started HTTP POST Request for Adding Coupon");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Coupon coupon = new Coupon
            {
                CouponCode = coup.CouponCode,
                Discount = coup.Discount
            };

            try
            {
                db.Coupons.Add(coupon);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                this._logger.Error("Exception in CouponsController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success HTTP POST Request for Adding Coupon");

            return CreatedAtRoute("DefaultApi", new { id = coupon.CouponId }, coupon);
        }
        
        // DELETE: api/Coupons/5
        [ResponseType(typeof(Coupon))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteCoupon(int id)
        {
            this._logger.Info("Starting HTTP DELETE Request for Deleting Coupon");

            Coupon coupon = db.Coupons.Find(id);

            if (coupon == null)
            {
                return NotFound();
            }

            try
            {
                db.Coupons.Remove(coupon);
                db.SaveChanges();
            }
            catch (Exception e)
            {

                this._logger.Error("Exception in CouponsController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success HTTP DELETE Request for Deleting Coupon");

            return Ok(coupon);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CouponExists(int id)
        {
            return db.Coupons.Count(e => e.CouponId == id) > 0;
        }
    }
}