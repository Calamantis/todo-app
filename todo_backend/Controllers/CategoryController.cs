using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todo_backend.Data;
using todo_backend.Models;
using System.Security.Claims;
using todo_backend.Services.CategoryService;
using todo_backend.Dtos.Category;
using Microsoft.Data.SqlClient.DataClassification;

namespace todo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //GET przeglądanie (swoich) kategorii
        [Authorize]
        [HttpGet("browse-categories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> BrowseCategories() {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var categories = await _categoryService.GetCategoriesAsync(userId);
            return Ok(categories);

        }


        //POST dodawanie kategorii
        [Authorize]
        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategory(CategoryDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.CreateCategoryAsync(dto, userId);
            return Ok(category);

        }

        //PUT modyfikowanie kategorii
        [Authorize]
        [HttpPut("update-category")]
        public async Task<IActionResult> UpdateCategory(CategoryDto dto, int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.UpdateCategoryAsync(dto, id, userId);
            if (category == null) return NotFound();

            return Ok(category);
        }

        //DELETE usuwanie kategorii
        [Authorize]
        [HttpDelete("delete-category")]
        public async Task<IActionResult> DeleteCategory(int id, [FromQuery] bool deleteActivities = false)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var currentUserId))
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var success = await _categoryService.DeleteCategoryAsync(id, deleteActivities, userId);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
