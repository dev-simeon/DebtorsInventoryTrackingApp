using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Middleware;

namespace WebApi.Controllers
{
    [Tags("Debts")]
    [Route("api/debts")]
    public class DebtController(ControllerParameters services) : SecureControllerBase(services)
    {
        // GET: api/debt
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DebtResponse>>> GetAllDebts(
            [FromQuery] PaginationQuery? pagination)
        {
            var query = Context.Debts
                .Include(d => d.Debtor)
                .Where(d => d.Debtor.UserId == CurrentUserId);

            return Ok(await ToPagedListAsync(query, pagination, debt => new DebtResponse(debt)));
        }

        // GET: api/debt/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DebtResponse>> GetDebt([FromRoute] string id)
        {
            var debt = await Context.Debts
                .Include(d => d.Debtor)
                .SingleOrDefaultAsync(d => d.Id == id && d.Debtor.UserId == CurrentUserId)
                ?? throw new NotFoundException("Debt not found.");

            return Ok(new DebtResponse(debt));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DebtorResponse>> ExtendDebtDueDate(string id, [FromBody] ExtendDebtDueDateRequest request)
        {
            var debt = await Context.Debts
                .Include(d => d.Debtor)
                .SingleOrDefaultAsync(d => d.Id == id && d.Debtor.UserId == CurrentUserId)
                ?? throw new NotFoundException("Debt not found.");

            debt.ExtendDueDate(request.ExtendDays);

            await Context.SaveChangesAsync();

            return Ok(new DebtResponse(debt));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDebt([FromRoute] string id)
        {
            var debt = await Context.Debts
                .Include(d => d.Debtor)
                .FirstOrDefaultAsync(d => d.Id == id && d.Debtor.UserId == CurrentUserId)
                ?? throw new NotFoundException("Payment not found.");

            Context.Debts.Remove(debt);
            await Context.SaveChangesAsync();

            return NoContent();
        }

    }
}
