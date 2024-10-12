using AutoMapper;
using Azure;
using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.UnitOfWork;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.Common;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using IfinionBackendAssessment.Service.ImageService;
using Microsoft.AspNetCore.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace IfinionBackendAssessment.Service.ProductServices
{
    public class ProductService:IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IphotoService _iphotoService;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IphotoService iphotoService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _iphotoService = iphotoService;
            _mapper = mapper;
        }
        public async Task<ApiResponse<ProductResponseDto>> AddProduct(AddProductDto productDto, IFormFile? Image)
        {
           var response = new ApiResponse<ProductResponseDto>();

            if (productDto == null) return new ApiResponse<ProductResponseDto> { IsSuccessful = false, Message = "Kindly provide data needed" };

            var productCategory = await _unitOfWork.CategoryRepo.GetCategoryByName(productDto.CategoryName);
            if (productCategory is null)
            {
                response.IsSuccessful = false;
                response.Message = "Category name does not exist";
                response.Errors = new string[] { "Product not added" };
                return response;
            }

            var imageUrl = string.Empty;
            if (Image is not null && Image.Length > 0)
            {
                var imageResult = await _iphotoService.AddPhotoAsync(Image);
                imageUrl = imageResult.Url.ToString();
            }

            var productToAdd = _mapper.Map<AddProductDto, Product>(productDto);
            productToAdd.CategoryId = productCategory.Id;
            productToAdd.Category = productCategory;
            productToAdd.ImageUrl = imageUrl;
            productToAdd.Count = productDto.Quantity;
            productToAdd.DateCreated = DateTime.UtcNow;
            productToAdd.DateUpdated = DateTime.UtcNow;

            var isAdded = await _unitOfWork.ProductRepo.AddAsync(productToAdd);

            if (isAdded)
            {
                var addedProduct = _mapper.Map<Product, ProductResponseDto>(productToAdd);
                response.IsSuccessful = true;
                response.Message = "Product Added successfully";
                response.Data = addedProduct;
                response.StatusCode = 000;

                return response;
            }

            response.IsSuccessful = false;
            response.Message = "An error occured while adding the product";
            response.Errors = new string[] { "Product Not Added" };
            return response;
        }

        public async Task<ApiResponse<ProductResponseDto>> UpdateProduct(UpdateProductDto updateProductDto,int id, IFormFile? Image)
        {
            if (id < 1) 
                return new ApiResponse<ProductResponseDto> { IsSuccessful = false, Message = "Invalid product id" };
            var imageUrl = string.Empty;
            var fileToUploadExist = Image is not null && Image.Length > 0;

            if (fileToUploadExist)
            {
                var imageResult = await _iphotoService.AddPhotoAsync(Image!);
                imageUrl = imageResult?.Url?.ToString();
            }
            var isProductUpdated = await _unitOfWork.ProductRepo.Update(updateProductDto, id, imageUrl);

            if (isProductUpdated.Id > 0)
            {
                var ProductToreturn = _mapper.Map<ProductResponseDto>(isProductUpdated);
                return new ApiResponse<ProductResponseDto>
                {
                    Message = "Product successfully updated",
                    IsSuccessful = true,
                    StatusCode = 000,
                    Data = ProductToreturn
                };
            }

            return new ApiResponse<ProductResponseDto>
            {
                Message = "Product update failed",
                IsSuccessful = false,
                StatusCode = 500,
                Errors = new string[] { "Product Update Failed" }
            };

        }

        public async Task<ApiResponse<ProductResponseDto>> GetProductById(int Id)
        {
            var Product = await _unitOfWork.ProductRepo.GetById(Id);
            if (Product == null) return new ApiResponse<ProductResponseDto> { Message = "No product found", IsSuccessful = false, Errors = new string[] { "Product Not Found" } };

            var ProductToReturn = _mapper.Map<ProductResponseDto>(Product);

            return new ApiResponse<ProductResponseDto>
            {
                Message = "Product Successfully fetched",
                Data = ProductToReturn,
                IsSuccessful = true
            };
        }
        public async Task<ApiResponse<string>> DeleteProduct(int Id)
        {
            var productToDelete = _unitOfWork.ProductRepo.GetFirstOrDefauly(x => x.Id == Id);
            if (productToDelete != null)
            {
                var isDeleted = await _unitOfWork.ProductRepo.RemoveAsync(productToDelete);
                if (!isDeleted) return new ApiResponse<string> { IsSuccessful = true, Message = "Opps! Product could not be deleted", Errors = new string[] { "Product Not Found" } };
                return new ApiResponse<string> { IsSuccessful = true, Message = "Product deleted successfully" };
            }

            return new ApiResponse<string> { IsSuccessful = false, Message = "Oops!! Product does not exist" };
        }

        public PagedResponse<ProductResponseDto> GetAllProducts(SearchParameter searchQuery)
        {
            if (searchQuery.MaxPrice < 0 || searchQuery.MinPrice < 0) return new PagedResponse<ProductResponseDto> { IsSuccessful = false, Message = "Product price cannot be a negative value" };
            var productsToReturn = new PagedResponse<ProductResponseDto>();

            var Products = _unitOfWork.ProductRepo.GetProductsWithSearch(searchQuery.Query, searchQuery.MinPrice, searchQuery.MaxPrice).Paginate(searchQuery.PageNumber, searchQuery.PageSize);
            if (Products.Result.Count < 1) return new PagedResponse<ProductResponseDto> { Message = "No product found", Errors = new string[] { "Product Not Found" } };

            foreach (var prod in Products.Result)
            {
                var product = _mapper.Map<ProductResponseDto>(prod);

                productsToReturn.Result.Add(product);
            }

            productsToReturn.IsSuccessful = true;
            productsToReturn.Message = "Successfully fetched all products";

            return productsToReturn;
        }

    }
}
