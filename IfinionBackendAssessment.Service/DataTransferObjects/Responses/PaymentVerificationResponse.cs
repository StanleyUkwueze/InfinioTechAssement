using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.DataTransferObjects.Responses
{
    public class PaymentVerificationResponse
    {
        public int amount { get; set; }
        public string? currency { get; set; }
        public DateTime transactionDate { get; set; }
        public string? status { get; set; }
        public string? reference { get; set; }
    }
}
