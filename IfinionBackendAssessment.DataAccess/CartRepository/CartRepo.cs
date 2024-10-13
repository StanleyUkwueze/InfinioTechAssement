using AutoMapper;
using Azure;
using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.Repository;
using IfinionBackendAssessment.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IfinionBackendAssessment.DataAccess.CartRepository
{
    public class CartRepo : ICartRepo
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper ;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartRepo(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        private int GetUserId()
        {
            var user = _httpContextAccessor.HttpContext!.User;
            if (user is null) return 0;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId is null ? 0 : int.Parse(userId!);
        }

        public ShoppingCart? GetCart(int userId)
        {
            var cart = _context.ShoppingCarts.Include(c => c.CartDetails).FirstOrDefault(x => x.CustomerId == userId);
            return cart;
        }
        public ApiResponse<int> AddToCart(int productId)
        {
            var userId = GetUserId();
            var successCount = 0;

            try
            {
                var userCart = GetCart(userId);
                if (userCart == null)
                {
                    userCart = new ShoppingCart
                    {
                        Status = true,
                        CustomerId = userId
                    };
                    _context.ShoppingCarts.Add(userCart);
                    if (_context.SaveChanges() > 0)
                    {
                        successCount++;
                    }
                }

                var CartDetail = new CartDetail();

                var prod = _context.Products.Find(productId);
                if (prod != null)
                {
                    if(prod.Count < 1) 
                        return new ApiResponse<int>
                        { 
                            Message = "Item is out of stock",
                            IsSuccessful = false, 
                            Data = GetCartItemCount(userId)
                        };
                
                    var existingCartDetail = userCart.CartDetails!.FirstOrDefault(x => x.ProductId == prod.Id);
                    if (existingCartDetail is not null)
                    {
                        existingCartDetail.Quantity++;
                    }
                    else
                    {
                        CartDetail = new()
                        {
                            ProductId = productId,
                            Quantity = 1,
                            Price = prod.Price,
                            ShoppingCartId = userCart.Id,
                            ProductName = prod.Name,
                            ImageUrl = prod.ImageUrl,
                        };
                        _context.CartDetails.Add(CartDetail);
                        _context.SaveChanges();
                    }

                    prod.Count--;
                    if (prod.Count < 1)
                    {
                        prod.InStock = false;

                    }
                    userCart.TotalPrice += prod.Price;

                    _context.ShoppingCarts.Update(userCart);
                    _context.Products.Update(prod);
                    if (_context.SaveChanges() > 0)
                    {
                        successCount++;
                    }

                   
                    if (successCount >= 1) 
                        return new ApiResponse<int> 
                        { 
                            Message = "Item added successfully",
                            Data = GetCartItemCount(userId),
                            IsSuccessful = true 
                        };
                    else
                        return new ApiResponse<int> 
                        { 
                            Message = $"Item not addition failed",
                            IsSuccessful = false 
                        };  
                }
                else
                {
                    return new ApiResponse<int>
                    {
                        Message = $"Item not added successfully: No product was found with the provided Id: {productId}",
                        IsSuccessful = false
                    };

                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return new ApiResponse<int>
            {
                Message = "Oops! Item could not be added to the Cart",
                IsSuccessful = false
            };
        }

        public async Task<ApiResponse<ShoppingCartResponse>> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == 0) return new ApiResponse<ShoppingCartResponse> { Message = "Invalid userid", IsSuccessful = false };

            var shoppingCart = await _context.ShoppingCarts
                                  .Include(a => a.CartDetails)
                                  .Where(a => a.CustomerId == userId).FirstOrDefaultAsync();
            if (shoppingCart == null) return new ApiResponse<ShoppingCartResponse> { Message = "User has no shoppingCart at the moment.", IsSuccessful = false};

            var response = _mapper.Map<ShoppingCartResponse>(shoppingCart);
            response.ItemsCount = shoppingCart.CartDetails!.Sum(x => x.Quantity);
            return new ApiResponse<ShoppingCartResponse> { IsSuccessful = true, Data = response };

        }

        public async Task<ApiResponse<int>> RemoveFromCart(int productId)
        {
            int userId = GetUserId();

            try
            {
                if (userId == 0) return new ApiResponse<int> { Message = "user is not logged-in", IsSuccessful = false };

                var cart = GetCart(userId);
                if (cart is null)
                    return new ApiResponse<int> { Message = "User has no cart tied to him.", IsSuccessful = false };
                var cartItems = await _context.CartDetails
                                  .Where(a => a.ShoppingCartId == cart.Id && a.ProductId == productId).ToListAsync();
                if (cartItems is null || cartItems.Count < 1) return new ApiResponse<int> { Message = "Item not found in shoppingcart", IsSuccessful = false, Data = GetCartItemCount(userId) };

                cart.TotalPrice -= cartItems.Sum(x => x.Price);

                foreach (var cartItem in cartItems)
                {
                    cart.CartDetails!.Remove(cartItem);
                }

                _context.CartDetails.RemoveRange(cartItems);

                _context.ShoppingCarts.Update(cart);
                _context.CartDetails.UpdateRange(cartItems);

                var productToUpdate = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
                if (productToUpdate is not null)
                {
                    productToUpdate.Count += cartItems.Sum(x => x.Quantity);
                    productToUpdate.InStock = true;
                    _context.Products.Update(productToUpdate);
                }
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return new ApiResponse<int>
            {
                Message = "Item Removed Successfully",
                Data = GetCartItemCount(userId),
                IsSuccessful = true
            };

        }


        public async Task<ApiResponse<int>> UpdateItemQuantity(int productId, bool isIncrement)
        {
            int userId = GetUserId();
            try
            {
                if (userId == 0) return new ApiResponse<int> { Message = "user is not logged-in", IsSuccessful = false };

                var cart = GetCart(userId);
                if (cart is null)
                    return new ApiResponse<int> { Message = "User has no cart tied to him.", IsSuccessful = false };
                var cartItem = await _context.CartDetails
                                  .FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.ProductId == productId);
                if (cartItem is null) return new ApiResponse<int> { Message = "Item not found in shoppingcart", IsSuccessful = false };
                var productToUpdate = await _context.Products.FirstOrDefaultAsync(x => x.Id == productId);
                if (productToUpdate is null) return new ApiResponse<int> { Message = "No product found", IsSuccessful = false };

                if (isIncrement)
                {
                    await UpdateEntities(_context, cart, cartItem, productToUpdate, isIncrement);

                    return new ApiResponse<int>
                    {
                        Message = "Item Successfully decreamented",
                        Data = GetCartItemCount(userId),
                        IsSuccessful = true
                    };
                }

                if (cartItem.Quantity == 1)
                {
                    return new ApiResponse<int> { Message = "You cannot decreament the quanity of the item as the quantity is already 1", IsSuccessful = false };
                }
                else
                {
                    await UpdateEntities(_context, cart, cartItem, productToUpdate, isIncrement);

                    return new ApiResponse<int>
                    {
                        Message = "Item Successfully decreamented",
                        Data = GetCartItemCount(userId),
                        IsSuccessful = true
                    };
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return new ApiResponse<int>
            {
                Message = "Oops! An error occured",
                Data = GetCartItemCount(userId),
                IsSuccessful = false
            };

        }

        private static async Task UpdateEntities(AppDbContext _context, ShoppingCart? cart, CartDetail? cartItem, Product? productToUpdate, bool increament)
        {
            if (increament)
            {
                cartItem!.Quantity++;

                cart!.TotalPrice += cartItem.Price;

                productToUpdate!.Count--;
            }
            else
            {
                cartItem!.Quantity--;

                cart!.TotalPrice -= cartItem.Price;

                productToUpdate!.Count++;
                productToUpdate.InStock = true;
            }

            _context.Products.Update(productToUpdate);
            _context.CartDetails.Update(cartItem);
            _context.ShoppingCarts.Update(cart);

            await _context.SaveChangesAsync();
        }

        public int GetCartItemCount(int userId)
        {

            var data = _context.ShoppingCarts.Include(d => d.CartDetails).Where(c => c.CustomerId == userId).FirstOrDefault();
            var counter = 0;
            foreach (var item in data!.CartDetails!)
            {
                counter += item.Quantity;
            }
            return counter!;
        }
    }
}
