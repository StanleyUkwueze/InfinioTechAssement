using IfinionBackendAssessment.DataAccess.CartRepository;
using IfinionBackendAssessment.Entity.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IfinionBackendAssessment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartRepo _cartRepo) : ControllerBase
    {

        [Authorize(Roles = Roles.Customer)]
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            var result = _cartRepo.AddToCart(productId);
            return Ok(result);
        }


        [Authorize(Roles = Roles.Customer)]
        [HttpDelete("{itemId}")]
        public async Task<IActionResult> RemoveFromCart(int itemId)
        {
            var result = await _cartRepo.RemoveFromCart(itemId);
            return Ok(result);
        }

        [Authorize(Roles = Roles.Customer)]
        [HttpGet]
        public async Task<IActionResult> ViewCart()
        {
            var result = await _cartRepo.GetUserCart();
            return Ok(result);
        }


        [Authorize(Roles = Roles.Customer)]
        [HttpGet("{itemId}")]
        public async Task<IActionResult> UpdateCartItem(int itemId, bool isIncreament)
        {
            var result = await _cartRepo.UpdateItemQuantity(itemId, isIncreament);
            return Ok(result);
        }
    }
}
