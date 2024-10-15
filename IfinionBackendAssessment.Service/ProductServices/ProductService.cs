using AutoMapper;
using Azure;
using IfinionBackendAssessment.DataAccess.CategoryRepository;
using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.ProductRepository;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.Common;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using IfinionBackendAssessment.Service.ImageService;
using Microsoft.AspNetCore.Http;

namespace IfinionBackendAssessment.Service.ProductServices
{
    public class ProductService:IProductService
    {
        private readonly IphotoService _iphotoService;
        private readonly IProductRepo _productRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IMapper _mapper;
        public ProductService(IphotoService iphotoService, IProductRepo productRepo, ICategoryRepo categoryRepo, IMapper mapper)
        {
            _iphotoService = iphotoService;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }
        public async Task<APIResponse<ProductResponseDto>> AddProduct(AddProductDto productDto, IFormFile? Image)
        {
           var response = new APIResponse<ProductResponseDto>();

            if (productDto == null) return new APIResponse<ProductResponseDto> { IsSuccessful = false, Message = "Kindly provide data needed" };

            var productCategory = await _categoryRepo.GetCategoryByName(productDto.CategoryName);
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

            var isAdded = await _productRepo.AddAsync(productToAdd);

            if (isAdded)
            {
                var addedProduct = _mapper.Map<Product, ProductResponseDto>(productToAdd);
                response.IsSuccessful = true;
                response.Message = "Product Added successfully";
                response.Data = addedProduct;

                return response;
            }

            response.IsSuccessful = false;
            response.Message = "An error occured while adding the product";
            response.Errors = new string[] { "Product Not Added" };
            return response;
        }

        public async Task<APIResponse<ProductResponseDto>> UpdateProduct(UpdateProductDto updateProductDto,int id, IFormFile? Image)
        {
            if (id < 1) 
                return new APIResponse<ProductResponseDto> { IsSuccessful = false, Message = "Invalid product id" };
            var imageUrl = string.Empty;
            var fileToUploadExist = Image is not null && Image.Length > 0;

            if (fileToUploadExist)
            {
                var imageResult = await _iphotoService.AddPhotoAsync(Image!);
                imageUrl = imageResult?.Url?.ToString();
            }
            var isProductUpdated = await _productRepo.Update(updateProductDto, id, imageUrl);

            if (isProductUpdated.Id > 0)
            {
                var ProductToreturn = _mapper.Map<ProductResponseDto>(isProductUpdated);
                return new APIResponse<ProductResponseDto>
                {
                    Message = "Product successfully updated",
                    IsSuccessful = true,
                    Data = ProductToreturn
                };
            }

            return new APIResponse<ProductResponseDto>
            {
                Message = "Product update failed",
                IsSuccessful = false,
                Errors = new string[] { "Product Update Failed" }
            };

        }

        public async Task<APIResponse<ProductResponseDto>> GetProductById(int Id)
        {
            var Product = await _productRepo.GetById(Id);
            if (Product == null) return new APIResponse<ProductResponseDto> { Message = "No product found", IsSuccessful = false, Errors = new string[] { "Product Not Found" } };

            var ProductToReturn = _mapper.Map<ProductResponseDto>(Product);

            return new APIResponse<ProductResponseDto>
            {
                Message = "Product Successfully fetched",
                Data = ProductToReturn,
                IsSuccessful = true
            };
        }
        public async Task<APIResponse<string>> DeleteProduct(int Id)
        {
            var productToDelete = _productRepo.GetFirstOrDefauly(x => x.Id == Id);
            if (productToDelete != null)
            {
                var isDeleted = await _productRepo.RemoveAsync(productToDelete);
                if (!isDeleted) return new APIResponse<string> { IsSuccessful = true, Message = "Opps! Product could not be deleted", Errors = new string[] { "Product Not Found" } };
                return new APIResponse<string> { IsSuccessful = true, Message = "Product deleted successfully" };
            }

            return new APIResponse<string> { IsSuccessful = false, Message = "Oops!! Product does not exist" };
        }

        public PagedResponse<ProductResponseDto> GetAllProducts(SearchParameter searchQuery)
        {
            if (searchQuery.MaxPrice < 0 || searchQuery.MinPrice < 0) return new PagedResponse<ProductResponseDto> { IsSuccessful = false, Message = "Product price cannot be a negative value" };
            var productsToReturn = new PagedResponse<ProductResponseDto>();

            var Products = _productRepo.GetProductsWithSearch(searchQuery.Query, searchQuery.MinPrice, searchQuery.MaxPrice).Paginate(searchQuery.PageNumber, searchQuery.PageSize);
            if (Products is null || Products.Result.Count < 1) return new PagedResponse<ProductResponseDto> { Message = "No product found", Errors = new string[] { "Product Not Found" } };

            foreach (var prod in Products.Result)
            {
                var product = _mapper.Map<ProductResponseDto>(prod);

                productsToReturn.Result.Add(product);
            }

            productsToReturn.IsSuccessful = true;
            productsToReturn.Message = "Successfully fetched all products";
            productsToReturn.TotalRecords = Products.Result.Count;
            productsToReturn.PageSize =searchQuery.PageSize;
            productsToReturn.CurrentPage = searchQuery.PageNumber;
            return productsToReturn;
        }

    }
}
