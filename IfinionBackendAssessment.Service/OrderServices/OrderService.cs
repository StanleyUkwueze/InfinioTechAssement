using AutoMapper;
using IfinionBackendAssessment.DataAccess;
using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.DataAccess.UserRepository;
using IfinionBackendAssessment.Entity.Constants;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Entity.Enums;
using IfinionBackendAssessment.Service.Common;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using IfinionBackendAssessment.Service.MailService;
using IfinionBackendAssessment.Service.TransactionServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Mail;
using static IfinionBackendAssessment.Service.MailService.EMailService;

namespace IfinionBackendAssessment.Service.OrderServices
{
    public class OrderService
        (
        HelperMethods helperMethods,
        AppDbContext _context,
        IUserRepository userRepository,
        IEMailService eMailService,
        IMapper mapper,
        ITransactionService transactionService
        ) : IOrderService
    {
        private Random random = new();
        public async Task<APIResponse<CheckoutResponse>> PlaceOrderAsync(PlaceOrderRequestModel PlaceOrderRequestModel)
        {
            var response = new CheckoutResponse();
            var result = new APIResponse<CheckoutResponse>();

            var emailRespone = string.Empty;
            try
            {
                var userId = helperMethods.GetUserId();

                var shoppingCart = await helperMethods.GetUserCart(userId.Item1);

                var scope = _context.Database.BeginTransaction();
                var order = new Order
                {
                    CustomerId = userId.Item1,
                    TotalPrice = shoppingCart.TotalPrice,
                    OrderStatus = OrderStatus.Processing.ToString(),
                    TrackingId = GenerateTrackingId(),
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

                foreach (var cartItem in shoppingCart.CartDetails)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price,
                        OrderId = order.Id,
                        ImageUrl = cartItem.ImageUrl
                    };
                    order.OrderItems.Add(orderItem);
                    _context.OrderItems.Add(orderItem);
                }

                _context.Orders.Update(order);
                if (!await helperMethods.Save()) return new APIResponse<CheckoutResponse> { Message = "Order failed to be placed", IsSuccessful = false };

                //payment
                var transactionDto = new TransactionDto
                {
                    Name = PlaceOrderRequestModel.Name,
                    Email = PlaceOrderRequestModel.Email,
                    Amount = shoppingCart.TotalPrice,
                    OrderId = order.Id,
                };
                var transaction = await transactionService.InitiatePayment(transactionDto, userId.Item1);

                if (!transaction.IsSuccessful)
                {
                    scope.Rollback();
                    return new APIResponse<CheckoutResponse> { Data = response, Message = "Transaction failed", IsSuccessful = false };
                }

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
                    To = Emails.AdminEmail,
                    Subject = EmailSubjects.OrderPlcacementNotifcation
                };


                emailRespone = await eMailService.NotifyAdminOfOrderPlacement(orderDetails, emailMessage);
                // notify the customer via email service
                emailMessage = new EmailMessage
                {
                    To = PlaceOrderRequestModel.Email!,
                    Subject = EmailSubjects.OrderPlcacementNotifcation
                };

                emailRespone = await eMailService.NotifyCustomerOfOrderStatus(emailMessage, "placed", order.TrackingId);
                return new APIResponse<CheckoutResponse> { Data = response, Message = "Order successfully placed", IsSuccessful = true };
            }
            catch (Exception ex)
            {
                result.Message = string.IsNullOrEmpty(emailRespone) ? $"Order successfully placed but order placement notification failed: {ex.Message}" : $"An error occured: {ex.Message}";
                result.IsSuccessful = string.IsNullOrEmpty(emailRespone) ? false : true;
                result.Data = response;
            }
           return result;
        }


        public async Task<APIResponse<OrderResponseDto>> GetUserOrderAsync(int orderId)
        {
            var userId = helperMethods.GetUserId();
            var order = new Order();

            if (userId.Item2 == Roles.Admin)
            {
                order = await _context.Orders.Include(x => x.OrderItems)
                      .FirstOrDefaultAsync(x =>
                      x.Id == orderId && x.OrderStatus != OrderStatus.Delivered.ToString());
            }
            else
            {
                order = await _context.Orders.Include(x => x.OrderItems)
                    .FirstOrDefaultAsync(x =>
                    x.Id == orderId
                    && x.CustomerId == userId.Item1 && x.OrderStatus != OrderStatus.Delivered.ToString()
                    );
            }
            if (order is null) return new APIResponse<OrderResponseDto>
            {
                IsSuccessful = false,
                Message = "No order record found"
            };

            var orderToReturn = mapper.Map<OrderResponseDto>(order);

            return new APIResponse<OrderResponseDto>
            {
                IsSuccessful = true,
                Message = "Successfully fetched order record",
                Data = orderToReturn
            };

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
           

            if (myOrders is null || myOrders.Count < 1)
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

        public async Task<APIResponse<OrderResponseDto>> EditOrderStatus(int orderId, string status)
        {
            Order? order = null;
            var emailResponse = string.Empty;
            var emailMessage = new EmailMessage();
            var orderToReturn = new OrderResponseDto();
            var result = new APIResponse<OrderResponseDto>();

            try
            {
                var orderStatusValidation = OrderStatusValidations(_context, orderId, status, out order);

                if (!orderStatusValidation.IsSuccessful) return orderStatusValidation;

                if (status.ToLower() == OrderStatus.Delivered.ToString().ToLower())
                {
                    var customer = await userRepository.GetUserById(order.CustomerId);

                    if (customer is not null)
                    {
                         emailMessage = new EmailMessage
                        {
                            Subject = EmailSubjects.OrderDeliveryNotification,
                            To = customer.Email
                        };
                        await eMailService.NotifyCustomerOfOrderStatus(emailMessage, status, order.TrackingId);
                    }

                    order.DateDelivered = DateTime.Now;
                }
                if (status.ToLower() == OrderStatus.Shipped.ToString().ToLower())
                {
                    var customer = await userRepository.GetUserById(order.CustomerId);

                    if (customer is not null)
                    {
                         emailMessage = new EmailMessage
                        {
                            Subject = EmailSubjects.OrderShipmentNotification,
                            To = customer.Email
                        };
                    
                    }
                    order.DateShipped = DateTime.Now;

                }
                order.OrderStatus = status;
                order.DateUpdated = DateTime.Now;

                _context.Orders.Update(order);
                var update = await _context.SaveChangesAsync();

                if (update > 0)
                {
                    orderToReturn = mapper.Map<OrderResponseDto>(order);
                    emailResponse = await eMailService.NotifyCustomerOfOrderStatus(emailMessage, status, order.TrackingId);
                    return new APIResponse<OrderResponseDto> { IsSuccessful = true, Data = orderToReturn, Message = "Order status Successfully updated" };
                }
               
                return new APIResponse<OrderResponseDto> { IsSuccessful = false, Message = "Order status update failed" };
            }
            catch (Exception ex)
            {
                orderToReturn = mapper.Map<OrderResponseDto>(order);
                result.Message = !string.IsNullOrEmpty(emailResponse) ? $"An error occured: {ex.Message}": $"Order status successfully updated but order status notification email failed: {ex.Message}";
                result.IsSuccessful = false;
                result.Data = orderToReturn;
            }
            return result;
        }

        private APIResponse<OrderResponseDto> OrderStatusValidations(AppDbContext _context, int orderId, string status, out Order? order)
        {
            var response = new APIResponse<OrderResponseDto> { IsSuccessful = true };
          
            order =  _context.Orders.FirstOrDefault(x => x.Id == orderId);

            if (order == null) return new APIResponse<OrderResponseDto> { IsSuccessful = false, Message = $"Order {orderId} is not found." };
            if (!Enum.IsDefined(typeof(OrderStatus), status))
                return new APIResponse<OrderResponseDto>
                {
                    IsSuccessful = false,
                    Message = $"Invalid orderStatus. Valid status: " +
                    $"{OrderStatus.Processing.ToString()}, {OrderStatus.Delivered.ToString()}," +
                    $" {OrderStatus.Shipped.ToString()}"
                };

            if (status.ToLower() == OrderStatus.Processing.ToString().ToLower())
            {
                return new APIResponse<OrderResponseDto>
                {
                    Message = $"An order is of status {OrderStatus.Processing.ToString()} by default",
                    IsSuccessful = false
                };
            }

            if (order.OrderStatus.ToLower() == OrderStatus.Delivered.ToString().ToLower()
                &&
                (status.ToLower() == OrderStatus.Delivered.ToString().ToLower()
                || status.ToLower() == OrderStatus.Shipped.ToString().ToLower()
                || status.ToLower() == OrderStatus.Processing.ToString().ToLower()
                ))
                return new APIResponse<OrderResponseDto>
                {
                    Message = "This order had already been marked delivered",
                    IsSuccessful = false
                };

            if (order.OrderStatus.ToLower() == OrderStatus.Shipped.ToString().ToLower()
               &&
               (status.ToLower() == OrderStatus.Shipped.ToString().ToLower()
               || status.ToLower() == OrderStatus.Processing.ToString().ToLower()
               )
               ) return new APIResponse<OrderResponseDto> { Message = "This order had already been marked shipped", IsSuccessful = false };

            if (order.OrderStatus.ToLower() == OrderStatus.Processing.ToString().ToLower()
                && status.ToLower() == OrderStatus.Delivered.ToString().ToLower())
            {

                response.IsSuccessful = false;
                response.Message = "This order is yet to be marked as shipped, hence cannot be marked as delivered";
                
            }
            return response;
        }

        private string GenerateTrackingId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] trackingId = new char[15];

            for (int i = 0; i < trackingId.Length; i++)
            {
                trackingId[i] = chars[random.Next(chars.Length)];
            }

            return new string(trackingId);
        }
    }
}
