using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;

namespace IfinionBackendAssessment.DataAccess.CartRepository
{
    public interface ICartRepo
    {
        APIResponse<int> AddToCart(int productId);
        Task<APIResponse<ShoppingCartResponse>> GetUserCart();
        Task<APIResponse<int>> RemoveFromCart(int productId);
        Task<APIResponse<int>> UpdateItemQuantity(int productId, bool isIncrement);
    }
}