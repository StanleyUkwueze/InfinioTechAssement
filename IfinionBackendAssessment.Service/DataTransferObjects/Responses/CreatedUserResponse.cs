using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.DataTransferObjects.Responses
{
    public class CreatedUserResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
