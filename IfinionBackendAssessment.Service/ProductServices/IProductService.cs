using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using Microsoft.AspNetCore.Http;

namespace IfinionBackendAssessment.Service.ProductServices
{
    public interface IProductService
    {
        Task<APIResponse<ProductResponseDto>> AddProduct(AddProductDto productDto, IFormFile? Image);
        Task<APIResponse<string>> DeleteProduct(int Id);
        PagedResponse<ProductResponseDto> GetAllProducts(SearchParameter searchQuery);
        Task<APIResponse<ProductResponseDto>> GetProductById(int Id);
        Task<APIResponse<ProductResponseDto>> UpdateProduct(UpdateProductDto updateProductDto, int Id, IFormFile? Image);
    }
}
