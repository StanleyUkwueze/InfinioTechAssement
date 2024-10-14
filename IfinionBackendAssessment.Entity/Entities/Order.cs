using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Entity.Entities
{
    public class Order:BaseEntity
    {
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsPaid { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Town { get; set; }
        public string? Street { get; set; }
        public string TrackingId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? DateCancelled { get; set; }
        public DateTime? DateShipped { get; set; }
        public DateTime? DateDelivered { get; set; }
        public List<OrderItem> OrderItems { get; set; } = [];

    }
}
