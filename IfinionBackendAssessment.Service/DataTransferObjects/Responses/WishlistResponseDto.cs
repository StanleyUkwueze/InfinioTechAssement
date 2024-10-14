using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.DataTransferObjects.Responses
{
    public class WishlistResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public string? ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }
    }
}
