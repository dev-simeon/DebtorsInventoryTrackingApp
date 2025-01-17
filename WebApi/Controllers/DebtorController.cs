using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Middleware;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Tags("Debtors")]
    [Route("api/debtors")]
    public class DebtorController(ControllerParameters services) : SecureControllerBase(services)
    {
        // GET: api/debtors/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DebtorResponse>> GetDebtor([FromRoute] string id)
        {
            var debtor = await Context.Debtors
                .SingleOrDefaultAsync(d => d.Id == id && d.UserId == CurrentUserId)
                ?? throw new NotFoundException("Debtor not found.");

            return Ok(new DebtorResponse(debtor));
        }

        // GET: api/debtors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DebtorResponse>>> GetDebtors(
            [FromQuery] PaginationQuery? pagination)
        {
            var query = Context.Debtors
                .Where(u => u.UserId == CurrentUserId);

            return Ok(await ToPagedListAsync(query, pagination, debtor => new DebtorResponse(debtor)));
        }

        // POST: api/debtors
        [HttpPost]
        public async Task<ActionResult<DebtorResponse>> CreateDebtor([FromBody] CreateDebtorRequest dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid debtor data.");

            var currentUser = await Context.Users.FindAsync(CurrentUserId)
                ?? throw new UnauthorizedAccessException();

            var address = new Address(dto.Street, dto.City, dto.State, dto.ZipCode);
            var debtor = new Debtor(dto.Name, dto.Phone, dto.Email, address);

            currentUser.Debtors.Add(debtor);
            await Context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDebtor), new { id = debtor.Id }, new DebtorResponse(debtor));
        }

        // PUT: api/debtors/{id}/contact-info
        [HttpPut("{id}/contact-info")]
        public async Task<ActionResult<DebtorResponse>> UpdateDebtorContactInfo(
            [FromRoute] string id, [FromBody] UpdateDebtorContactInfoRequest dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid contact info.");

            var debtor = await Context.Debtors
                .SingleOrDefaultAsync(d => d.Id == id && d.UserId == CurrentUserId)
                ?? throw new NotFoundException("Debtor not found.");

            debtor.UpdateContactInfo(dto.Phone, dto.Email);
            await Context.SaveChangesAsync();

            return Ok(new DebtorResponse(debtor));
        }

        // PUT: api/debtors/{id}/address
        [HttpPut("{id}/address")]
        public async Task<ActionResult<DebtorResponse>> UpdateDebtorAddress(
            [FromRoute] string id, [FromBody] UpdateDebtorAddressRequest dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid address data.");

            var debtor = await Context.Debtors
                .SingleOrDefaultAsync(d => d.Id == id && d.UserId == CurrentUserId)
                ?? throw new NotFoundException("Debtor not found.");

            var address = new Address(dto.Street, dto.City, dto.State, dto.ZipCode);

            debtor.UpdateAddress(address);
            await Context.SaveChangesAsync();

            return Ok(new DebtorResponse(debtor));
        }

        // DELETE: api/debtors/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<DebtorResponse>> DeleteDebtor([FromRoute] string id)
        {
            var debtor = await Context.Debtors
                .SingleOrDefaultAsync(d => d.Id == id && d.UserId == CurrentUserId)
                ?? throw new NotFoundException("Debtor not found.");

            Context.Debtors.Remove(debtor);
            await Context.SaveChangesAsync();

            return Ok(new DebtorResponse(debtor));
        }

        [HttpPost("{id}/debts")]
        public async Task<ActionResult<DebtResponse>> AddDebtToDebtor([FromRoute] string id, [FromBody] CreateDebtRequest dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid debt data.");

            var debtor = await Context.Debtors
                .Include(d => d.Debts)
                .SingleOrDefaultAsync(d => d.Id == id && d.UserId == CurrentUserId)
                ?? throw new NotFoundException("Debtor not found.");

            var debt = new Debt(dto.TotalAmount, dto.DueDate, dto.AmountOwed);
            debtor.AddDebt(debt);

            await Context.SaveChangesAsync();

            return Ok(new DebtResponse(debt));
        }

        [HttpDelete("{debtorId}/debts/{debtId}")]
        public async Task<ActionResult<DebtResponse>> RemoveDebtFromDebtor([FromRoute] string debtorId, [FromRoute] string debtId)
        {
            var debtor = await Context.Debtors
                .Include(d => d.Debts)
                .SingleOrDefaultAsync(d => d.Id == debtorId && d.UserId == CurrentUserId)
                ?? throw new NotFoundException("Debtor not found.");

            var debt = debtor.Debts.FirstOrDefault(x => x.Id == debtId)
                ?? throw new NotFoundException("Debt not found.");

            debtor.RemoveDebt(debt);
            await Context.SaveChangesAsync();

            return Ok(new DebtResponse(debt));
        }

        [HttpGet("{id}/debts")]
        public async Task<ActionResult<IEnumerable<DebtResponse>>> GetDebtorDebts([FromRoute] string id)
        {
            var debtor = await Context.Debtors
                .Include(d => d.Debts)
                .SingleOrDefaultAsync(d => d.Id == id && d.UserId == CurrentUserId)
                ?? throw new NotFoundException("Debtor not found.");

            var debts = debtor.Debts.Select(d => new DebtResponse(d));
            return Ok(debts);
        }

    }
}
