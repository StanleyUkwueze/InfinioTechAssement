using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.DataTransferObjects.Responses
{
    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ProductResponseDto>? Products { get; set; }
    }
}
