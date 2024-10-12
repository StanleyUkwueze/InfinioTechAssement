using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using Microsoft.AspNetCore.Http;

namespace IfinionBackendAssessment.Service.ProductServices
{
    public interface IProductService
    {
        Task<ApiResponse<ProductResponseDto>> AddProduct(AddProductDto productDto, IFormFile? Image);
        Task<ApiResponse<string>> DeleteProduct(int Id);
        PagedResponse<ProductResponseDto> GetAllProducts(SearchParameter searchQuery);
        Task<ApiResponse<ProductResponseDto>> GetProductById(int Id);
        Task<ApiResponse<ProductResponseDto>> UpdateProduct(UpdateProductDto updateProductDto, int Id, IFormFile? Image);
    }
}
