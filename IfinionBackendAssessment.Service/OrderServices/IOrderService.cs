using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;

namespace IfinionBackendAssessment.Service.OrderServices
{
    public interface IOrderService
    {
        Task<APIResponse<CheckoutResponse>> PlaceOrderAsync(PlaceOrderRequestModel PlaceOrderRequestModel);
        Task<APIResponse<OrderResponseDto>> EditOrderStatus(int orderId, string status);
        Task<APIResponse<List<OrderResponseDto>>> GetUserOrders();
        Task<APIResponse<OrderResponseDto>> GetUserOrderAsync(int orderId);
    }
}