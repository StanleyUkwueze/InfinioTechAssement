namespace IfinionBackendAssessment.Entity.Entities
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public bool InStock { get; set; }
        public Category Category { get; set; } = new();
    }
}
