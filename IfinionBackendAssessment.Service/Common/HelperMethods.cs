using IfinionBackendAssessment.DataAccess;
using IfinionBackendAssessment.Entity.Constants;
using IfinionBackendAssessment.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.Common
{
    public class HelperMethods(IHttpContextAccessor _httpContextAccessor, AppDbContext _context)
    {
        public (int,string) GetUserId()
        {
            var user = _httpContextAccessor.HttpContext!.User;
            
            if (user is null) return (0,"");
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return (0, "");

            var role = user.IsInRole(Roles.Admin) ? Roles.Admin : Roles.Customer;

            return (int.Parse(userId!), role);
        }

        public async Task<ShoppingCart> GetUserCart(int userId)
        {
            var shoppingCart = await _context.ShoppingCarts
                                       .Include(sc => sc.CartDetails)!
                                       .FirstOrDefaultAsync(sc => sc.CustomerId == userId);
            return shoppingCart!;
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
