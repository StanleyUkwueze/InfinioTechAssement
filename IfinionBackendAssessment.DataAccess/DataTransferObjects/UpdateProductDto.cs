namespace IfinionBackendAssessment.DataAccess.DataTransferObjects
{
    public class UpdateProductDto
    {
        public string? Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public string? Description { get; set; } = string.Empty;
    }
}
