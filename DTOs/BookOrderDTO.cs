using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStoreBackend.DTOs
{
    public class BookOrderDTO
    {
        public int OrderId { get; set; }
        public System.DateTime OrderDate { get; set; }
        public int? CouponId { get; set; }
        public int UserId { get; set; }

    }

    public class BookQuantity
    {
        public int? BookId { get; set; }
        public int? Qty { get; set; }
        public int? ItemPrice { get; set; }
    }
       

    public class BookListOrderDTO
    {
        public System.DateTime OrderDate { get; set; }
        public int? CouponId { get; set; }
        public int? UserId { get; set; }
        public ICollection<BookQuantity> BookList { get; set; }
    }
}