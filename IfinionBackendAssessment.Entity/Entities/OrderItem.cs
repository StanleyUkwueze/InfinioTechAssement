using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Entity.Entities
{
    public class OrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string? ImageUrl { get; set; }
        public int OrderId { get; set; }
        public decimal Price { get; set; }
    }
}
