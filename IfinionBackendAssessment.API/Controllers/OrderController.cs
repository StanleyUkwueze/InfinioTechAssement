using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Entity.Constants;
using IfinionBackendAssessment.Service.OrderServices;
using IfinionBackendAssessment.Service.TransactionServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IfinionBackendAssessment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService, ITransactionService transactionService) : ControllerBase
    {
        [Authorize(Roles = Roles.Customer)]
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] PlaceOrderRequestModel placeOrderRequestModel)
        {
            var result = await orderService.PlaceOrderAsync(placeOrderRequestModel);
            return Ok(result);
        }

        [Authorize(Roles = $"{Roles.Customer},{Roles.Admin}")]
        [HttpGet("{orderId}")]
        public async Task<IActionResult> ViewOrder(int orderId)
        {
            var result = await orderService.GetUserOrderAsync(orderId);
            return Ok(result);
        }


        [Authorize(Roles = $"{Roles.Customer},{Roles.Admin}")]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var result = await orderService.GetUserOrders();
            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> MarkOrderAsDelivere(int orderId, string status)
        {
            var result = await orderService.EditOrderStatus(orderId, status);
            return Ok(result);
        }
        [HttpGet("verify-payment")]
        public async Task<IActionResult> VerifyPayment(string trxref)
        {
            var response = await transactionService.VerifyPayment(trxref);

            if (response.status!.Contains("success")) return Ok(response);


            return BadRequest(response);
        }
    }
}
