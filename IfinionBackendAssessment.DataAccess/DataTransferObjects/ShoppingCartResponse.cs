using IfinionBackendAssessment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.DataAccess.DataTransferObjects
{
    public class ShoppingCartResponse
    {
        public int Id { get; set; }
        public int ItemsCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public List<CartDetail>? CartDetails { get; set; } = [];
    }
}
