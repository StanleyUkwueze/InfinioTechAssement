using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.DataAccess.WishListRepository
{
    public class WishListRepo : GenericRepository<Wishlist>, IWishListRepo
    {
        private readonly AppDbContext _context;
        public WishListRepo(AppDbContext context): base(context) 
        {
            _context = context;
        }

        public async Task<Wishlist> GetWishlistByItemId(int itemId)
        {
            var item = await _context.Wishlists.Where(x => x.ProductId == itemId).FirstOrDefaultAsync();
            return item!;
        }
    }
}
