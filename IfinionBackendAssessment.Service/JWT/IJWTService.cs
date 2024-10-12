using IfinionBackendAssessment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.JWT
{
    public interface IJWTService
    {
        Task<string> GenerateJwtToken(User user);
    }
}
