using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Entity.Entities
{
    public class Transaction:BaseEntity
    {
        public string CustomerName { get; set; }
        public int Amount { get; set; }
        public string TrxRef { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? OrderTrackingId { get; set; }
        public bool Status { get; set; }
    }
}
