using AutoMapper;
using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.ProductRepository;
using IfinionBackendAssessment.DataAccess.WishListRepository;
using IfinionBackendAssessment.Entity.Constants;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.CacheService;
using IfinionBackendAssessment.Service.Common;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IfinionBackendAssessment.Service.WishListServices
{
    public class WishListService
        (
        IWishListRepo wishListRepo,
        ICacheService<Wishlist> cacheService,
        IProductRepo productRepo,
        IConfiguration configuration,
        HelperMethods helperMethods,
        IMapper mapper
        ) : IWishListService
    {
        string CacheKey = $"{configuration.GetSection("CacheSettings:CacheKey").Value!}_WishLists";
        public async Task<APIResponse<WishlistResponseDto>> AddWishListItem(AddWishlistDto addWishlistDto)
        {
            var response = new WishlistResponseDto();
            if (addWishlistDto.ProductId <= 0) 
                return new APIResponse<WishlistResponseDto> 
                { 
                    IsSuccessful = false,
                    Message = "Invalid item id" 
                };
            var item = await productRepo.GetById(addWishlistDto.ProductId);

            if (item is null)
                return new APIResponse<WishlistResponseDto>
                { 
                    IsSuccessful = false,
                    Message = "No item found" 
                };
            var user = helperMethods.GetUserId();

            if(user.Item1 == 0 || string.IsNullOrEmpty(user.Item2))
                return new APIResponse<WishlistResponseDto>
                {
                    IsSuccessful = false,
                    Message = "Invalid user identity"
                };

            if(user.Item2 == Roles.Admin)
                return new APIResponse<WishlistResponseDto>
                {
                    IsSuccessful = false,
                    Message = "UnAuthorized: Admin User"
                };

            var wishItem = await wishListRepo.GetWishlistByItemId(addWishlistDto.ProductId);
            var isSaved = false;

            if (wishItem is not null)
            {
                wishItem.Quantity++;
                isSaved = await wishListRepo.UpdateAsync(wishItem);
            }
            else
            {
                wishItem = new Wishlist
                {
                    ImageUrl = item.ImageUrl,
                    Quantity = 1,
                    Price = item.Price,
                    ProductName = item.Name,
                    CustomerId = user.Item1,
                    ProductId = addWishlistDto.ProductId
                };

               isSaved = await wishListRepo.AddAsync(wishItem);
            }

            if (isSaved)
            {
                response = mapper.Map<WishlistResponseDto>(wishItem);
                return new APIResponse<WishlistResponseDto> 
                {
                    Data = response,
                    IsSuccessful = true,
                    Message = "Successfully added item to wishlist"
                };
            }

            return new APIResponse<WishlistResponseDto>
            {
                IsSuccessful = false,
                Message = "Adding item to wishlist failed"
            };
        }

        public async Task<APIResponse<List<WishlistResponseDto>>> GetAllWishlistItems()
        {
            var wishlistItems = new List<Wishlist>();
            var response = new List<WishlistResponseDto>();

            var user = helperMethods.GetUserId();

            if (user.Item1 == 0 || string.IsNullOrEmpty(user.Item2))
                return new APIResponse<List<WishlistResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "Invalid user identity"
                };

            if (user.Item2 == Roles.Admin)
                return new APIResponse<List<WishlistResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "UnAuthorized: Admin User"
                };

            wishlistItems = cacheService.GetDataFromCacheAsyc(CacheKey);
            if(wishlistItems is not null && wishlistItems.Count > 0)
            {
                response = mapper.Map<List<WishlistResponseDto>>(wishlistItems);
                return new APIResponse<List<WishlistResponseDto>>
                {
                    IsSuccessful = true,
                    Message = "Successfully fetched wishlist items",
                    Data = response
                };
            }
             wishlistItems = await wishListRepo.GetAll().Where(x => x.CustomerId == user.Item1).ToListAsync();

            if(wishlistItems is null || wishlistItems.Count <= 0)
                return new APIResponse<List<WishlistResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "You do not have an item in wishlist"
                };
            response = mapper.Map<List<WishlistResponseDto>>(wishlistItems);
            cacheService.SaveToCache(wishlistItems, CacheKey);

            return new APIResponse<List<WishlistResponseDto>>
            {
                IsSuccessful = true,
                Message = "Successfully fetched wishlist items",
                Data = response
            };
        }

        public async Task<APIResponse<string>> RemoveItem(int itemId)
        {
            if (itemId <= 0)
                return new APIResponse<string>
                {
                    IsSuccessful = false,
                    Message = "Invalid item id"
                };
            var user = helperMethods.GetUserId();

            if (user.Item1 == 0 || string.IsNullOrEmpty(user.Item2))
                return new APIResponse<string>
                {
                    IsSuccessful = false,
                    Message = "Invalid user identity"
                };

            if (user.Item2 == Roles.Admin)
                return new APIResponse<string>
                {
                    IsSuccessful = false,
                    Message = "UnAuthorized: Admin User"
                };
            var itemToRemove = await wishListRepo.GetAll()
                              .FirstOrDefaultAsync(x => x.CustomerId == user.Item1 && x.ProductId == itemId);

            if (itemToRemove is null)
                return new APIResponse<string>
                {
                    IsSuccessful = false,
                    Message = "No item found to remove"
                };
            var isSaved = await wishListRepo.RemoveAsync(itemToRemove);

            if (isSaved)
            {
                return new APIResponse<string>
                {
                    IsSuccessful = true,
                    Message = "Item successfully removed from wishlist"
                };
            }
            return new APIResponse<string>
            {
                IsSuccessful = false,
                Message = "An error occurred"
            };
        }
    }
}
