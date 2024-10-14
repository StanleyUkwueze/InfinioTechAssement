using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.CategoryServices
{
    public interface ICategoryService
    {
        Task<APIResponse<AddCategoryResponse>> AddCategory(AddCategoryDto categoryDto);
        Task<APIResponse<CategoryResponseDto>> UpdateCategory(UpdateCategoryDto updateCategoryDto, int id);
       Task<APIResponse<string>> DeleteCategory(int Id);
    }
}
