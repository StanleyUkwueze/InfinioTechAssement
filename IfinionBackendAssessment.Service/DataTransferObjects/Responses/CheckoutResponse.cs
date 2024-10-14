using IfinionBackendAssessment.Entity.Entities;

namespace IfinionBackendAssessment.Service.DataTransferObjects.Responses
{
    public class CheckoutResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public string TrackingId { get; set; }
        public bool IsPaid { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Town { get; set; }
        public string? Street { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? DateCreated { get; set; }
        public string AuthorizationUrl { get; set; }
        public string Reference { get; set; }
        public List<OrderItem> OrderItems { get; set; } = [];
    }
}
