using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Service.CategoryServices;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IfinionBackendAssessment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService _categoryService) : ControllerBase
    {
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromForm] AddCategoryDto categoryDto)
        {
            var result = await _categoryService.AddCategory(categoryDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            return Ok(result);
        }

        //[Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCategory([FromForm] UpdateCategoryDto categoryDto, int id)
        {
            var result = await _categoryService.UpdateCategory(categoryDto, id);
            return Ok(result);
        }
    }
}
