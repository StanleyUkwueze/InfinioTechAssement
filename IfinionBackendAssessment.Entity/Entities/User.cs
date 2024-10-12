using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Entity.Entities
{
    public class User:BaseEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PaswordHash { get; set; }
        public string Role { get; set; }
    }
}
