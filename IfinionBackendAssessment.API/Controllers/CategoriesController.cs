using IfinionBackendAssessment.DataAccess.DataTransferObjects;
using IfinionBackendAssessment.Entity.Constants;
using IfinionBackendAssessment.Service.CategoryServices;
using IfinionBackendAssessment.Service.DataTransferObjects.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IfinionBackendAssessment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(ICategoryService _categoryService) : ControllerBase
    {
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromForm] AddCategoryDto categoryDto)
        {
            var result = await _categoryService.AddCategory(categoryDto);
            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCategory([FromForm] UpdateCategoryDto categoryDto, int id)
        {
            var result = await _categoryService.UpdateCategory(categoryDto, id);
            return Ok(result);
        }
    }
}
