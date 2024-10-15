

namespace IfinionBackendAssessment.DataAccess.DataTransferObjects
{
    public class SearchParameter
    {
        public string? Query { get; set; }
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}
