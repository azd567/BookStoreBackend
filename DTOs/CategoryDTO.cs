using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStoreBackend.DTOs
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public byte[] Image { get; set; }
        public int Position { get; set; }
    }
}