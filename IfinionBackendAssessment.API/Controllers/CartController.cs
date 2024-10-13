using IfinionBackendAssessment.DataAccess.CartRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IfinionBackendAssessment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartRepo _cartRepo) : ControllerBase
    {
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var result = _cartRepo.AddToCart(productId);
            return Ok(result);
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            var result = await _cartRepo.RemoveFromCart(itemId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            var result = await _cartRepo.GetUserCart();
            return Ok(result);
        }

        [HttpGet("{itemId}")]
        public async Task<IActionResult> UpdateCartItem(int itemId, bool isIncreament)
        {
            var result = await _cartRepo.UpdateItemQuantity(itemId, isIncreament);
            return Ok(result);
        }
    }
}
