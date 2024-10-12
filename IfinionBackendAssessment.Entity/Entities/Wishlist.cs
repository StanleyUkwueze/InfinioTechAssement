

namespace IfinionBackendAssessment.Entity.Entities
{
    public class Wishlist: BaseEntity
    {
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public string? ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }
    }
}
