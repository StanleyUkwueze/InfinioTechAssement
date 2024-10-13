using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Entity.Entities
{
    public class ShoppingCart: BaseEntity
    {
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Status { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartDetail> CartDetails { get; set; } = [];
    }
}
