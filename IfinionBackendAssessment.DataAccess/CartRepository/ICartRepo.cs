using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;

namespace IfinionBackendAssessment.DataAccess.CartRepository
{
    public interface ICartRepo
    {
        ApiResponse<int> AddToCart(int productId);
        Task<ApiResponse<ShoppingCartResponse>> GetUserCart();
        Task<ApiResponse<int>> RemoveFromCart(int productId);
        Task<ApiResponse<int>> UpdateItemQuantity(int productId, bool isIncrement);
    }
}