﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookStoreBackend.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class bookstoreDBEntities : DbContext
    {
        public bookstoreDBEntities()
            : base("name=bookstoreDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookOrder> BookOrders { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Coupon> Coupons { get; set; }
        public virtual DbSet<OrdersBooks> OrdersBooks { get; set; }
    
        public virtual ObjectResult<GetOrdersByUser_Result> GetOrdersByUser(Nullable<int> userId)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetOrdersByUser_Result>("GetOrdersByUser", userIdParameter);
        }
    
        public virtual ObjectResult<Search_Result> Search(string param, string val)
        {
            var paramParameter = param != null ?
                new ObjectParameter("param", param) :
                new ObjectParameter("param", typeof(string));
    
            var valParameter = val != null ?
                new ObjectParameter("val", val) :
                new ObjectParameter("val", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Search_Result>("Search", paramParameter, valParameter);
        }
    
        public virtual int SetCategoryStatus(Nullable<int> categoryId, Nullable<bool> setStatus)
        {
            var categoryIdParameter = categoryId.HasValue ?
                new ObjectParameter("categoryId", categoryId) :
                new ObjectParameter("categoryId", typeof(int));
    
            var setStatusParameter = setStatus.HasValue ?
                new ObjectParameter("setStatus", setStatus) :
                new ObjectParameter("setStatus", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SetCategoryStatus", categoryIdParameter, setStatusParameter);
        }
    
        public virtual int SetUserActivation(Nullable<int> userId, Nullable<bool> setActivation)
        {
            var userIdParameter = userId.HasValue ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(int));
    
            var setActivationParameter = setActivation.HasValue ?
                new ObjectParameter("setActivation", setActivation) :
                new ObjectParameter("setActivation", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SetUserActivation", userIdParameter, setActivationParameter);
        }
    }
}
