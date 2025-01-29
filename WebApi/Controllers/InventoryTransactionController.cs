using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Middleware;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Tags("Inventory Transactions")]
    [Route("api/inventory-transactions")]
    public class InventoryTransactionController(ControllerParameters services) : SecureControllerBase(services)
    {
        // GET: api/inventory-transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryTransactionResponse>>> GetAllTransactions([FromQuery] PaginationQuery? pagination)
        {
            var query = Context.InventoryTransactions
                .Include(t => t.Product)
                .Where(t => t.Product.OwnerId == CurrentUserId);

            return Ok(await ToPagedListAsync(query, pagination, transaction => new InventoryTransactionResponse(transaction)));
        }

        // GET: api/inventory-transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryTransactionResponse>> GetTransactionById([FromRoute] string id)
        {
            var transaction = await Context.InventoryTransactions
                .Include(t => t.Product)
                .FirstOrDefaultAsync(t => t.Id == id && t.Product.OwnerId == CurrentUserId)
                ?? throw new NotFoundException("Transaction not found.");

            return Ok(new InventoryTransactionResponse(transaction));
        }

        // GET: api/inventory-transactions/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<InventoryTransactionResponse>>> GetTransactionsByProduct([FromRoute] string productId, [FromQuery] PaginationQuery? pagination)
        {
            var query = Context.InventoryTransactions
                .Include(t => t.Product)
                .Where(t => t.Product.Id == productId && t.Product.OwnerId == CurrentUserId);

            return Ok(await ToPagedListAsync(query, pagination, transaction => new InventoryTransactionResponse(transaction)));
        }

        // POST: api/inventory-transactions/add-stock
        [HttpPost("add-stock")]
        public async Task<ActionResult<InventoryTransactionResponse>> AddStock([FromBody] InventoryTransactionRequest request)
        {
            var product = await Context.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.OwnerId == CurrentUserId)
                ?? throw new NotFoundException("Product not found.");

            var transaction = new InventoryTransaction(request.Quantity, "Stock Added", request.Notes);

            product.Update(product.Name, product.Description, product.UnitPrice, product.StockQuantity + request.Quantity);

            product.StockMovements.Add(transaction);
            await Context.SaveChangesAsync();

            return Ok(new InventoryTransactionResponse(transaction));
        }

        // POST: api/inventory-transactions/remove-stock
        [HttpPost("remove-stock")]
        public async Task<ActionResult<InventoryTransactionResponse>> RemoveStock([FromBody] InventoryTransactionRequest request)
        {
            var product = await Context.Products
                .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.OwnerId == CurrentUserId)
                ?? throw new NotFoundException("Product not found.");

            if (request.Quantity > product.StockQuantity)
                throw new InvalidOperationException("Cannot remove more stock than available.");

            var transaction = new InventoryTransaction(request.Quantity, "Stock Removed", request.Notes);

            product.Update(product.Name, product.Description, product.UnitPrice, product.StockQuantity - request.Quantity);

            product.StockMovements.Add(transaction);
            await Context.SaveChangesAsync();

            return Ok(new InventoryTransactionResponse(transaction));
        }

        // DELETE: api/inventory-transactions/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<InventoryTransactionResponse>> DeleteTransaction([FromRoute] string id)
        {
            var transaction = await Context.InventoryTransactions
                .Include(t => t.Product)
                .FirstOrDefaultAsync(t => t.Id == id && t.Product.OwnerId == CurrentUserId)
                ?? throw new NotFoundException("Transaction not found.");

            Context.InventoryTransactions.Remove(transaction);
            await Context.SaveChangesAsync();

            return Ok(new InventoryTransactionResponse(transaction));
        }
    }
}
