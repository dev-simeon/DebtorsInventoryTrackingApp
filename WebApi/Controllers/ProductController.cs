using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Middleware;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Tags("Products")]
    [Route("api/products")]
    public class ProductController(ControllerParameters services) : SecureControllerBase(services)
    {
        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProducts([FromQuery] PaginationQuery? pagination)
        {
            var query = Context.Products
                .Include(p => p.Category)
                .Where(p => p.OwnerId == CurrentUserId);

            return Ok(await ToPagedListAsync(query, pagination, product => new ProductResponse(product)));
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct([FromRoute] string id)
        {
            var product = await Context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == CurrentUserId)
                ?? throw new NotFoundException("Product not found.");

            return Ok(new ProductResponse(product));
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid product data.");

            var category = await Context.Categories
                .Include(c => c.Products.Where(p => p.OwnerId == CurrentUserId))
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId)
                ?? throw new NotFoundException("Category not found.");

            var product = new Product(request.Name, request.Description, request.UnitPrice, request.StockQuantity);

            category.Products.Add(product);
            await Context.SaveChangesAsync();

            return Ok(new ProductResponse(product));
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductResponse>> UpdateProduct([FromRoute] string id, [FromBody] CreateProductRequest request)
        {
            var product = await Context.Products
                .Include(x => x.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == CurrentUserId)
                ?? throw new NotFoundException("Product not found.");

            product.Update(request.Name, request.Description, request.UnitPrice, request.StockQuantity);
            await Context.SaveChangesAsync();

            return Ok(new ProductResponse(product));
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductResponse>> DeleteProduct([FromRoute] string id)
        {
            var product = await Context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == CurrentUserId)
                ?? throw new NotFoundException("Product not found.");

            Context.Products.Remove(product);
            await Context.SaveChangesAsync();

            return Ok(new ProductResponse(product));
        }
    }
}
