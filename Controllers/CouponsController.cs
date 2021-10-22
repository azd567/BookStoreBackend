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
using BookStoreBackend.Models;

namespace BookStoreBackend.Controllers
{
    public class CouponsController : ApiController
    {
        private bookstoreDBEntities db = new bookstoreDBEntities();

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
            // Console.WriteLine(coupon);
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
        public IHttpActionResult PostCoupon(Coupon coupon)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Coupons.Add(coupon);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = coupon.CouponId }, coupon);
        }
        // x
        // DELETE: api/Coupons/5
        [ResponseType(typeof(Coupon))]
        public IHttpActionResult DeleteCoupon(int id)
        {
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return NotFound();
            }

            db.Coupons.Remove(coupon);
            db.SaveChanges();

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