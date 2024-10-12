namespace IfinionBackendAssessment.Entity.Entities
{
    public class CartDetail: BaseEntity
    {
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }
        public int ShoppingCartId { get; set; }
    }
}