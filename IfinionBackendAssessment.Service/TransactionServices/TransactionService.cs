using AutoMapper;
using Azure;
using IfinionBackendAssessment.DataAccess;
using IfinionBackendAssessment.DataAccess.Common;
using IfinionBackendAssessment.Entity.Entities;
using IfinionBackendAssessment.Service.Common;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using IfinionBackendAssessment.Service.DataTransferObjects.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayStack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IfinionBackendAssessment.Service.TransactionServices
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly HelperMethods _helperMethods;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string token;
        private readonly string baseUrl;

        private PayStackApi Paystack { get; set; }
        public TransactionService(HttpClient httpClient, IMapper mapper, HelperMethods helperMethods, IConfiguration configuration, AppDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
            _mapper = mapper;   
            _helperMethods = helperMethods;
            _configuration = configuration;
            token = _configuration["Payment:PaystackSK"]!;
            baseUrl = _configuration.GetSection("BaseUrl").Value!;
            Paystack = new PayStackApi(token);
        }

        public async Task<APIResponse<TransactionInitializeResponse>> InitiatePayment(TransactionDto transactionDto, int userId)
        {
            var order = new Order();
            TransactionInitializeRequest request = new()
            {
                AmountInKobo = ((int)(transactionDto.Amount * 100)),
                Email = transactionDto.Email,
                Reference = ReferenceGenerator().ToString(),
                Currency = "NGN",
                CallbackUrl = $"{baseUrl}/api/Orders/verify-payment"
            };

            if (userId == 0)
            {
                return new APIResponse<TransactionInitializeResponse> { Message = "Invalid user", IsSuccessful = false };
            }

            order = await _context.Orders
                    .FirstOrDefaultAsync(x =>
                    x.Id ==
                    transactionDto.OrderId
                    && x.CustomerId == userId
                    && !x.IsPaid);

            if (order is null)
            {
                return new APIResponse<TransactionInitializeResponse>
                {
                    Message = $"No Order record found",
                    IsSuccessful = false
                };
            }
            
            var response = Paystack.Transactions.Initialize(request);
            response.RawJson = null;
            if (response.Status)
            {

                _context.Orders.Update(order!);

                var transaction = new Transaction()
                {
                    Amount = (int)transactionDto.Amount,
                    Email = transactionDto.Email!,
                    TrxRef = request.Reference,
                    CustomerName = transactionDto.Name!,
                    OrderId = transactionDto.OrderId
                };

                await _context.Transactions.AddAsync(transaction);
                var isSaved = await _context.SaveChangesAsync();

                if (isSaved > 1)
                {
                    return new APIResponse<TransactionInitializeResponse>
                    {
                        Message = response.Message,
                        IsSuccessful = response.Status,
                        Data = response
                    };
                }

                return new APIResponse<TransactionInitializeResponse>
                {
                    Message = "Transaction/payment records not saved successfully",
                    IsSuccessful = false
                };
            }

            return new APIResponse<TransactionInitializeResponse>
            {
                Message = "Transaction failed",
                IsSuccessful = false
            };
        }

        public async Task<PaymentVerificationResponse> VerifyPayment(string trxref)
        {
            TransactionVerifyResponse response = Paystack.Transactions.Verify(trxref);
            var result = new PaymentVerificationResponse();

            if (response.Status)
            {
                var userId = _helperMethods.GetUserId();
                var transaction = _context.Transactions.Where(x => x.TrxRef == trxref).FirstOrDefault();
                if (transaction != null)
                {
                    var order = await _context.Orders
                       .FirstOrDefaultAsync(x =>
                       x.Id ==
                       transaction.OrderId
                       && x.CustomerId == userId.Item1
                       && !x.IsPaid);

                    if (order is not null)
                    {
                        order.IsPaid = true;
                        _context.Update(order);
                    }

                    transaction.Status = true;
                    _context.Transactions.Update(transaction);
                    await _context.SaveChangesAsync();

                    var jsonResponese = JsonConvert.SerializeObject(response.Data);
                    result = JsonConvert.DeserializeObject<PaymentVerificationResponse>(jsonResponese);

                    return result!;
                }
            }

            return result;
        }


        public static long ReferenceGenerator()
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            return rand.Next(100000000, 999999999);
        }
    }
}
