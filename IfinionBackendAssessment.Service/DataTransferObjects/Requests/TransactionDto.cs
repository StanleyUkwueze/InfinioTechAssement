

namespace IfinionBackendAssessment.Service.DataTransferObjects.Requests
{
    public class TransactionDto
    {
        public string? Name { get; set; }
        public decimal Amount { get; set; }
        public string? Email { get; set; }
        public int OrderId { get; set; }
    }
}
