using BookStoreBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStoreBackend.DTOs
{
    public class BookDTO
    {
        public Nullable<int> BookId { get; set; }

        public string Title { get; set; }

        public string ISBN { get; set; }

        public Nullable<short> Year { get; set; }

        public string Description { get; set; }

        public Nullable<bool> Status { get; set; }

        public byte[] Image { get; set; }

        public Nullable<int> Price { get; set; }

        public Nullable<int> Position { get; set; }

        public Nullable<int> Qty { get; set; }

        public Nullable<bool> Featured { get; set; }

        public string Author { get; set; }

        public string Category { get; set; }

        public Nullable<int> AuthorId { get; set; }

        public Nullable<int> CategoryId { get; set; }

        public BookDTO()
        {}

        public BookDTO(Book book)
        {
            BookId = book.BookId;
            Title = book.Title;
            ISBN = book.ISBN;
            Year = book.Year;
            Description = book.Description;
            Status = book.Status;
            Image = book.Image;
            Price = book.Price;
            Position = book.Position;
            Qty = book.Qty;
            Featured = book.Featured;
            Author = book.Author.AuthorName;
            Category = book.Category.Name;
        }
    }
}