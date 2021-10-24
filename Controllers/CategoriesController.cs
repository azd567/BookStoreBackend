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
        public IQueryable<CategoryDTO> GetCategories()
        {
            var Categories = from item in db.Categories
                             select new CategoryDTO
                             {
                                 CategoryId = item.CategoryId,
                                 Name = item.Name,
                                 CreatedAt = item.CreatedAt,
                                 Description = item.Description,
                                 Status = item.Status,
                                 Image = item.Image,
                                 Position = item.Position
                             };
            return Categories;
        }

        // GET: api/Categories/5
        [ResponseType(typeof(CategoryDTO))]
        public IHttpActionResult GetCategory(int id)
        {
            var category = db.Categories.Select(
                categories => new CategoryDTO()
                {
                    CategoryId = categories.CategoryId,
                    Name = categories.Name,
                    CreatedAt = categories.CreatedAt,
                    Description = categories.Description,
                    Status = categories.Status,
                    Image = categories.Image,
                    Position = categories.Position

                }).SingleOrDefault(b=> b.CategoryId== id);
                
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // PUT: api/Categories/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCategory(int id, Category category)
        {
            this._logger.Info("Started HTTP PUT Request for Updating Category");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            if (!CategoryExists(id))
            {
                return NotFound();
            }

            try
            {
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
        public IHttpActionResult PostCategory(Category category)
        {
            this._logger.Info("Started HTTP POST Request for Adding Category");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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