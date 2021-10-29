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
using BookStoreBackend.Models;
using BookStoreBackend.DTOs;
using BookStoreBackend.Interfaces;

namespace BookStoreBackend.Controllers
{
    public class CategoriesController : ApiController
    {
        private bookstoreDBEntities db = new bookstoreDBEntities();
        private readonly ILoggerService _logger;

        public CategoriesController(ILoggerService logger)
        {
            this._logger = logger;
        }

        // GET: api/Categories
        public IEnumerable<CategoryDTO> GetCategories()
        {
            return CategoryDTO.SerializeCategoryList(db.Categories);
        }

        // GET: api/Categories/5
        [ResponseType(typeof(CategoryDTO))]
        public IHttpActionResult GetCategory(int id)
        {
            var category = db.Categories.Find(id);
    
            if (category == null)
            {
                return NotFound();
            }

            return Ok(new CategoryDTO(category));
        }

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PutCategory(int id, CategoryDTO cat)
        {
            this._logger.Info("Started HTTP PUT Request for Updating Category");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (cat == null || cat.CategoryId == null)
            {
                return BadRequest("No Cateogry Id provided");
            }

            if(id != cat.CategoryId.Value)
            {
                return BadRequest("URI Dosen't match with perovided id");
            }    

            if (!CategoryExists(cat.CategoryId.Value))
            {
                return NotFound();
            }

            try
            {
                var category = db.Categories.Find(cat.CategoryId.Value);

                category.Name = cat.Name ?? category.Name;
                category.Description = cat.Description ?? category.Description;
                category.Status = cat.Status ?? category.Status;
                category.Image = cat.Image != null ? Convert.FromBase64String(cat.Image) : category.Image;
                category.Position = cat.Position ?? category.Position;

                db.Entry(category).State = EntityState.Modified;
            
                db.SaveChanges();
            }
            catch (Exception e)
            {
                this._logger.Error("Exception in CategoriesController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success in HTTP PUT Request for Updating Category");

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Categories
        [ResponseType(typeof(Category))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult PostCategory(CategoryDTO cat)
        {
            this._logger.Info("Started HTTP POST Request for Adding Category");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Category category = new Category{
                Name = cat.Name,
                CreatedAt = cat.CreatedAt,
                Description = cat.Description,
                Status = cat.Status == null || cat.Status.Value,
                Image = cat.Image != null ? Convert.FromBase64String(cat.Image) : null,
                Position = cat.Position.Value
            };
                
            try
            {
                db.Categories.Add(category);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                this._logger.Error("Exception in CategoriesController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success in HTTP POST Request for Adding Category");

            return CreatedAtRoute("DefaultApi", new { id = category.CategoryId }, category);
        }

        // DELETE: api/Categories/5
        [ResponseType(typeof(Category))]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteCategory(int id)
        {
            this._logger.Info("Started HTTP DELETE  Request for Deleting Category");

            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            try
            {
                db.Categories.Remove(category);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                this._logger.Error("Exception in CategoriesController : " + e.Message);
                return InternalServerError(e);
            }

            this._logger.Info("Success in HTTP DELETE  Request for Deleting Category");

            return Ok(category);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CategoryExists(int id)
        {
            return db.Categories.Count(e => e.CategoryId == id) > 0;
        }
    }
}