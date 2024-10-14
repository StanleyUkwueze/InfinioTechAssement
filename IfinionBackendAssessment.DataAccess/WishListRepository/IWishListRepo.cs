using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;

namespace IfinionBackendAssessment.DataAccess.WishListRepository
{
    public interface IWishListRepo : IGenericRepository<Wishlist>
    {
        Task<Wishlist> GetWishlistByItemId(int itemId);

    }
}