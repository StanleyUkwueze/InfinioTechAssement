using Azure;
using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Entity.Constants;
using IfinionBackendAssessment.Service.ProductServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IfinionBackendAssessment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductService _productService) : ControllerBase
    {
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDto productDto, IFormFile? Image)
        {
            var response = await _productService.AddProduct(productDto, Image);
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDto updateProductDto, int id, IFormFile? image)
        {
            var response = await _productService.UpdateProduct(updateProductDto, id, image);
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProduct(id);
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = await _productService.GetProductById(id);
            return response.IsSuccessful ? Ok(response) : BadRequest(response);
        }

        [HttpGet()]
        public IActionResult GetAllProducts([FromQuery] SearchParameter searchQuery)
        {
            var products = _productService.GetAllProducts(searchQuery);
            if (!products.IsSuccessful) return NotFound(products);
            return Ok(products);
        }
    }
}
