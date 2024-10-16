﻿using AutoMapper;
using Azure;
using IfinionBackendAssessment.DataAccess.CategoryRepository;
using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.ProductRepository;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.CacheService;
using IfinionBackendAssessment.Service.Common;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using IfinionBackendAssessment.Service.ImageService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace IfinionBackendAssessment.Service.ProductServices
{
    public class ProductService:IProductService
    {
        private readonly IphotoService _iphotoService;
        private readonly IProductRepo _productRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IConfiguration _configuration;
        private readonly ICacheService<Product> _cacheService;
        private readonly IMapper _mapper;
        public ProductService(
            IphotoService iphotoService,
            ICacheService<Product> cacheService,
            IConfiguration configuration,
            IProductRepo productRepo,
            ICategoryRepo categoryRepo,
            IMapper mapper)
        {
            _iphotoService = iphotoService;
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _configuration = configuration;
            _cacheService = cacheService;
            _mapper = mapper;
        }
        public async Task<APIResponse<ProductResponseDto>> AddProduct(AddProductDto productDto, IFormFile? Image)
        {
           var response = new APIResponse<ProductResponseDto>();
            var addedProduct = new ProductResponseDto();

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

            var productToAdd = await _productRepo.GetProductByName(productDto.Name);
            if(productToAdd is not null)
            {
                productToAdd.Count += productDto.Quantity;
                productToAdd.DateUpdated = DateTime.Now;

               if (await _productRepo.UpdateAsync(productToAdd))
                {
                    addedProduct = _mapper.Map<Product, ProductResponseDto>(productToAdd);
                    response.IsSuccessful = true;
                    response.Message = "This Product already exist: the Count/Quantity updated successfully";
                    response.Data = addedProduct;
                    return response;
                }
                response.IsSuccessful = false;
                response.Message = "An error occured while trying to update the count of this product as it already exist";
                response.Errors = new string[] { "Product quantity Not updated" };
                return response;

            }
            productToAdd = _mapper.Map<AddProductDto, Product>(productDto);
            productToAdd.CategoryId = productCategory.Id;
            productToAdd.Category = productCategory;
            productToAdd.ImageUrl = imageUrl;
            productToAdd.Count = productDto.Quantity;

            var isAdded = await _productRepo.AddAsync(productToAdd);

            if (isAdded)
            {
                addedProduct = _mapper.Map<Product, ProductResponseDto>(productToAdd);
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
            var CacheKey = $"{_configuration.GetSection("CacheSettings:CacheKey").Value!}_{Id}";
            var ProductToReturn = new ProductResponseDto();
           var productFromCache = _cacheService.GetDataFromCacheAsyc(CacheKey);
            if(productFromCache != null && productFromCache.Count > 0)
            {
                ProductToReturn = _mapper.Map<ProductResponseDto>(productFromCache[0]);
                return new APIResponse<ProductResponseDto>
                {
                    Message = "Product Successfully fetched",
                    Data = ProductToReturn,
                    IsSuccessful = true
                };
            }
            var Product = await _productRepo.GetById(Id);
            if (Product == null) return new APIResponse<ProductResponseDto> 
            { Message = "No product found", IsSuccessful = false,
                Errors = new string[] { "Product Not Found" } };

            _cacheService.SaveToCache(new List<Product> { Product }, CacheKey);

             ProductToReturn = _mapper.Map<ProductResponseDto>(Product);

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
            var productsToReturn = new PagedResponse<ProductResponseDto>();
            var CacheKey = $"{_configuration.GetSection("CacheSettings:CacheKey").Value!}";
 
            var productsFromCache = _cacheService.GetDataFromCacheAsyc(CacheKey);
            if (productsFromCache != null && productsFromCache.Count > 0)
            {
                foreach (var prod in productsFromCache)
                {
                    var product = _mapper.Map<ProductResponseDto>(prod);

                    productsToReturn.Result.Add(product);
                }
                productsToReturn.IsSuccessful = true;
                productsToReturn.Message = "Successfully fetched all products";
                productsToReturn.TotalRecords = productsFromCache.Count;
                productsToReturn.PageSize = searchQuery.PageSize;
                productsToReturn.CurrentPage = searchQuery.PageNumber;
                return productsToReturn;
            }

            if (searchQuery.MaxPrice < 0 || searchQuery.MinPrice < 0)
                return new PagedResponse<ProductResponseDto>
                { 
                    IsSuccessful = false,
                    Message = "Product price cannot be a negative value"
                };

            var Products = _productRepo.GetProductsWithSearch(searchQuery.Query, searchQuery.MinPrice, searchQuery.MaxPrice)
                .Paginate(searchQuery.PageNumber, searchQuery.PageSize);

            if (Products is null || Products.Result.Count < 1) 
                return new PagedResponse<ProductResponseDto> 
                { 
                    Message = "No product found", 
                    Errors = new string[] { "Product Not Found" }
                };

            foreach (var prod in Products.Result)
            {
                var product = _mapper.Map<ProductResponseDto>(prod);

                productsToReturn.Result.Add(product);
            }

            _cacheService.SaveToCache(Products.Result.ToList(), CacheKey);

            productsToReturn.IsSuccessful = true;
            productsToReturn.Message = "Successfully fetched all products";
            productsToReturn.TotalRecords = Products.Result.Count;
            productsToReturn.PageSize =searchQuery.PageSize;
            productsToReturn.CurrentPage = searchQuery.PageNumber;
            return productsToReturn;
        }

    }
}
