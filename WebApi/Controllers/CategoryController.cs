using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Middleware;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController(ControllerParameters services) : SecureControllerBase(services)
    {
        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories()
        {
            var categories = await Context.Categories
                .Include(x => x.Products.Where(x => x.OwnerId == CurrentUserId))
                .ToListAsync();

            return Ok(categories.Select(c => new CategoryResponse(c)));
        }

        // GET: api/categories/{categoryId}
        [HttpGet("{categoryId}")]
        public async Task<ActionResult<CategoryResponse>> GetCategory([FromRoute] string categoryId)
        {
            var category = await Context.Categories
                .Include(c => c.Products.Where(a => a.OwnerId == CurrentUserId))
                .FirstOrDefaultAsync(c => c.Id == categoryId)
                ?? throw new NotFoundException("Category not found");

            return Ok(new CategoryResponse(category));
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> CreateCategory([FromBody] CategoryCreateRequest categoryRequest)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid argument");

            // Create a new category
            var category = new Category(categoryRequest.Name, categoryRequest.Description);

            Context.Categories.Add(category);
            await Context.SaveChangesAsync();

            return Ok(new CategoryResponse(category));
        }

        // PUT: api/categories/{categoryId}
        [HttpPut("{categoryId}")]
        public async Task<ActionResult<CategoryResponse>> UpdateCategory([FromRoute] string categoryId,
            [FromBody] CategoryCreateRequest categoryRequest)
        {
            var category = await Context.Categories.FindAsync(categoryId);

            if (category == null)
                return NotFound(new { message = "Category not found" });

            // Update the category details
            category.UpdateCategory(categoryRequest.Name, categoryRequest.Description);
            await Context.SaveChangesAsync();

            return Ok(new CategoryResponse(category));
        }

        // DELETE: api/categories/{categoryId}
        [HttpDelete("{categoryId}")]
        public async Task<ActionResult<CategoryResponse>> DeleteCategory([FromRoute] string categoryId)
        {
            var category = await Context.Categories.FindAsync(categoryId);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            Context.Categories.Remove(category);
            await Context.SaveChangesAsync();

            return Ok(new CategoryResponse(category));
        }
    }
}