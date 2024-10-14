namespace IfinionBackendAssessment.Entity.Entities
{
    public class ShoppingCart: BaseEntity
    {
        public int CustomerId { get; set; }
        public bool Status { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartDetail> CartDetails { get; set; } = [];
    }
}
