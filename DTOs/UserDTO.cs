using BookStoreBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookStoreBackend.DTOs
{
    public class UserDTO
    {
        public Nullable<int> Id { get; set; }

        public string Name { get; set; }

        public Nullable<bool> IsActive { get; set; }

        public Nullable<bool> IsAdmin { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }

        public UserDTO()
        {}

        public UserDTO(AppUser appUser)
        {
            Id = appUser.UserId;
            Name = appUser.UserName;
            Address = appUser.UserAddress;
            IsActive = appUser.IsActive;
            IsAdmin = appUser.IsAdmin;
        }

        static public IQueryable<UserDTO> SerializeUserList(IQueryable<AppUser> userList)
        {
            return userList.Select(User => new UserDTO
            {
                Id = User.UserId,
                Name = User.UserName,
                Address = User.UserAddress,
                IsActive = User.IsActive,
                IsAdmin = User.IsAdmin
            });
        }
    }
}