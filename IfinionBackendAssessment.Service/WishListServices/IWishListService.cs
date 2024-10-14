using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;

namespace IfinionBackendAssessment.Service.WishListServices
{
    public interface IWishListService
    {
        Task<APIResponse<WishlistResponseDto>> AddWishListItem(AddWishlistDto addWishlistDto);
        Task<APIResponse<List<WishlistResponseDto>>> GetAllWishlistItems();
        Task<APIResponse<string>> RemoveItem(int itemId);
    }
}