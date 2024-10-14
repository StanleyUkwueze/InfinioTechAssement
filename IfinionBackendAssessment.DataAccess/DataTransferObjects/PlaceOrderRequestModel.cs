namespace IfinionBackendAssessment.DataAccess.DataTransferObjects
{
    public class PlaceOrderRequestModel
    {
        public string State { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
