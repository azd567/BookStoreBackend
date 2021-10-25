using BookStoreBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStoreBackend.DTOs
{
    public class CategoryDTO
    {
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public byte[] Image { get; set; }
        public int? Position { get; set; }

        public CategoryDTO()
        {
        }

        public CategoryDTO(Category category)
        {
            CategoryId = category.CategoryId;
            Name = category.Name;
            CreatedAt = category.CreatedAt;
            Description = category.Description;
            Status = category.Status;
            Image = category.Image;
            Position = category.Position;            
        }

        static public IQueryable<CategoryDTO> SerializeCategoryList(IQueryable<Category> categoryList)
        {
            return categoryList.Select(category => new CategoryDTO
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                Description = category.Description,
                Status = category.Status,
                Image = category.Image,
                Position = category.Position,
            });
        }
    }
}