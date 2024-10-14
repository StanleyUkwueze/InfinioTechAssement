using IfinionBackendAssessment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.DataTransferObjects.Responses
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ImageUrl { get; set; } = "";
        public string OrderStatus { get; set; }
        public bool IsPaid { get; set; }
        public DateTime DateCancelled { get; set; }
        public DateTime Delivered { get; set; }
        public DateTime Shipped { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
