
namespace IfinionBackendAssessment.DataAccess.DataTransferObjects
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Count { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated{get; set;}
    }
}
