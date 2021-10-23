using BookStoreBackend.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreBackend.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(int usreId, string userName, bool isAdmin, uint days = 1);
    }
}
