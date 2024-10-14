using AutoMapper;
using IfinionBackendAssessment.DataAccess;
using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Entity.Constants;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Entity.Enums;
using IfinionBackendAssessment.Service.Common;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using IfinionBackendAssessment.Service.MailService;
using IfinionBackendAssessment.Service.TransactionServices;
using Microsoft.EntityFrameworkCore;
using static IfinionBackendAssessment.Service.MailService.EMailService;

namespace IfinionBackendAssessment.Service.OrderServices
{
    public class OrderService(HelperMethods helperMethods, AppDbContext _context,IEMailService eMailService, IMapper mapper, ITransactionService transactionService) : IOrderService
    {
        public async Task<APIResponse<CheckoutResponse>> PlaceOrderAsync(PlaceOrderRequestModel PlaceOrderRequestModel)
        {
            var response = new CheckoutResponse();
            var userId = helperMethods.GetUserId();

            var shoppingCart = await helperMethods.GetUserCart(userId.Item1);

            var scope = _context.Database.BeginTransaction();
            var order = new Order
            {
                CustomerId = userId.Item1,
                TotalPrice = shoppingCart.TotalPrice,
                OrderStatus = OrderStatus.Processing.ToString(),
                State = PlaceOrderRequestModel.State,
                City = PlaceOrderRequestModel.City,
                Town = PlaceOrderRequestModel.Town,
                Street = PlaceOrderRequestModel.Street,
                IsPaid = false,

            };

            _context.Orders.Add(order);
            await helperMethods.Save();

            if (shoppingCart.CartDetails!.Count < 1)
                return new APIResponse<CheckoutResponse>
                { 
                    Message = "Kindly add items to your shopping cart before attempting placing an order",
                    IsSuccessful = false 
                };

            foreach(var cartItem in shoppingCart.CartDetails)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price,
                    OrderId = order.Id,
                    ImageUrl = cartItem.ImageUrl
                };

                //order.TotalPrice += orderItem.Price;
                order.OrderItems.Add(orderItem);
                _context.OrderItems.Add(orderItem);
            }

            _context.Orders.Update(order);
            if(!await helperMethods.Save()) return new APIResponse<CheckoutResponse> { Message = "Order failed to be placed", IsSuccessful = false };

            //payment
            var transactionDto = new TransactionDto
            {
                Name = PlaceOrderRequestModel.Name,
                Email = PlaceOrderRequestModel.Email,
                Amount = shoppingCart.TotalPrice,
                OrderId = order.Id,
            };
            var transaction = await transactionService.InitiatePayment(transactionDto, userId.Item1);

            if(transaction.IsSuccessful)
            {
                //remove items from the cart
                shoppingCart.CartDetails.RemoveRange(0, shoppingCart.CartDetails.Count);
                _context.CartDetails.RemoveRange(shoppingCart.CartDetails);
                shoppingCart.TotalPrice = 0;
                _context.ShoppingCarts.Update(shoppingCart);
                _context.SaveChanges();
                scope.Commit();

                response = mapper.Map<CheckoutResponse>(order);
                response.AuthorizationUrl = transaction.Data.Data.AuthorizationUrl;
                response.Reference = transaction.Data.Data.Reference;

                var orderDetails = mapper.Map<OrderDetails>(response);

                // notify the admin via email service
                var emailMessage = new EmailMessage
                {
                    To = "stanleyjekwu16@gmail.com",
                    Subject = "Order Placement Notification"
                };

                await eMailService.NotifyAdminOfOrderPlacement(orderDetails, emailMessage);
                
                return new APIResponse<CheckoutResponse> { Data = response, Message = "Order successfully placed", IsSuccessful = true };
            }
            return new APIResponse<CheckoutResponse> { Data = response, Message = "Transaction failed", IsSuccessful = false };

        }


        public async Task<Order> GetUserOrderAsync(int orderId)
        {
            var userId = helperMethods.GetUserId();
            var order = await _context.Orders.Include(x => x.OrderItems)
                         .FirstOrDefaultAsync(x =>
                         x.Id == orderId
                         && x.CustomerId == userId.Item1
                         );
            return order!;
        }

        public async Task<APIResponse<List<OrderResponseDto>>> GetUserOrders()
        {
            var userId = helperMethods.GetUserId();

            if (userId.Item1 == 0) return new APIResponse<List<OrderResponseDto>>
            {
                Message = "Invalid user",
                IsSuccessful = false
            };

            var myOrders = new List<Order>();
            if (userId.Item2 == Roles.Admin)
            {
             myOrders = await _context.Orders.Include(cd => cd.OrderItems)
                               .Where(x => x.OrderStatus != OrderStatus.Delivered.ToString())
                                     .ToListAsync();
            }
            else
            {
                 myOrders = await _context.Orders.Include(cd => cd.OrderItems)
                    .Where(x => x.CustomerId == userId.Item1
                          && x.OrderStatus != OrderStatus.Delivered.ToString())
                          .ToListAsync();
            }
           

            if (myOrders is null)
                return new APIResponse<List<OrderResponseDto>>
                {
                    IsSuccessful = false,
                    Message = "No Active order found"
                };
            var ordersToReturn = mapper.Map<List<OrderResponseDto>>(myOrders);
            return new APIResponse<List<OrderResponseDto>>
            {
                IsSuccessful = true,
                Message = "Orders successfully fetched",
                Data = ordersToReturn
            };
        }

        public async Task<APIResponse<string>> EditOrderStatus(int orderId, string status)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), status))
                return new APIResponse<string>
                { 
                    IsSuccessful = false, 
                    Message = $"Invalid orderStatus. Valid status: " +
                    $"{OrderStatus.Processing.ToString()},{OrderStatus.Delivered.ToString()}," +
                    $" {OrderStatus.Shipped.ToString()}" 
                };

            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order == null) return new APIResponse<string> { IsSuccessful = false, Message = $"Order {orderId} is not found." };


            if (order.OrderStatus.ToLower() == OrderStatus.Delivered.ToString().ToLower() 
                && 
                (status.ToLower() == OrderStatus.Delivered.ToString().ToLower() 
                || status.ToLower() == OrderStatus.Shipped.ToString().ToLower()
                || status.ToLower() == OrderStatus.Processing.ToString().ToLower()
                )) 
                return new APIResponse<string> 
                { 
                    Message = "This order had already been marked delivered",
                    IsSuccessful = false
                };

            if (order.OrderStatus.ToLower() == OrderStatus.Shipped.ToString().ToLower()
               &&
               (status.ToLower() == OrderStatus.Shipped.ToString().ToLower()
               || status.ToLower() == OrderStatus.Processing.ToString().ToLower()
               )
               ) return new APIResponse<string> { Message = "This order had already been marked shipped", IsSuccessful = false };

                if(status.ToLower() == OrderStatus.Delivered.ToString().ToLower())
                {
                    order.DateDelivered = DateTime.Now;
                }
                if (status.ToLower() == OrderStatus.Shipped.ToString().ToLower())
                {
                    order.DateShipped = DateTime.Now;
                }
                order.OrderStatus = status;
                
               _context.Orders.Update(order);
                var update = await _context.SaveChangesAsync();

                if (update > 0)
                {
                    return new APIResponse<string> { IsSuccessful = true, Message = "Order status Successfully updated" };
                }
                return new APIResponse<string> { IsSuccessful = false, Message = "Order status update failed" };
        }
    }
}
