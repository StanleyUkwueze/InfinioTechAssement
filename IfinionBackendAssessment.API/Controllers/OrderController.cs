using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Service.OrderServices;
using IfinionBackendAssessment.Service.TransactionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IfinionBackendAssessment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService, ITransactionService transactionService) : ControllerBase
    {
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] PlaceOrderRequestModel placeOrderRequestModel)
        {
            var result = await orderService.PlaceOrderAsync(placeOrderRequestModel);
            return Ok(result);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> ViewOrder(int orderId)
        {
            var result = await orderService.GetUserOrderAsync(orderId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var result = await orderService.GetUserOrders();
            return Ok(result);
        }

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
