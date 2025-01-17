using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DTOs;
using WebApi.Middleware;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Tags("Payments")]
    [Route("api/payments")]
    public class PaymentController(ControllerParameters services) : SecureControllerBase(services)
    {
        // GET: api/payments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentResponse>>> GetAllPayments([FromQuery] PaginationQuery? pagination)
        {
            var query = Context.Payments
                .Include(p => p.Debt)
                .ThenInclude(d => d.Debtor)
                .Where(p => p.Debt.Debtor.UserId == CurrentUserId);

            return Ok(await ToPagedListAsync(query, pagination, payment => new PaymentResponse(payment)));
        }

        // GET: api/payments/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentResponse>> GetPayment([FromRoute] string id)
        {
            var payment = await Context.Payments
                .Include(p => p.Debt)
                .ThenInclude(d => d.Debtor)
                .FirstOrDefaultAsync(p => p.Id == id && p.Debt.Debtor.UserId == CurrentUserId)
                ?? throw new NotFoundException("Payment not found.");

            return Ok(new PaymentResponse(payment));
        }

        // POST: api/payments
        [HttpPost]
        public async Task<ActionResult<PaymentResponse>> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid payment data.");

            var debt = await Context.Debts
                .Include(d => d.Debtor)
                .FirstOrDefaultAsync(d => d.Id == request.DebtId && d.Debtor.UserId == CurrentUserId)
                ?? throw new NotFoundException("Debt not found.");

            var payment = new Payment(request.Amount, request.PaymentMethod, request.Note);
            debt.RecordPayment(payment.Amount, payment.Note, payment.PaymentMethod);

            Context.Payments.Add(payment);
            await Context.SaveChangesAsync();

            return Ok(new PaymentResponse(payment));
        }

        // // PUT: api/payments/{id}
        // [HttpPut("{id}")]
        // public async Task<ActionResult<PaymentResponse>> UpdatePayment([FromRoute] string id, [FromBody] UpdatePaymentRequest request)
        // {
        //     if (!ModelState.IsValid)
        //         return BadRequest("Invalid payment data.");

        //     var payment = await Context.Payments
        //         .Include(p => p.Debt)
        //         .ThenInclude(d => d.Debtor)
        //         .FirstOrDefaultAsync(p => p.Id == id && p.Debt.Debtor.UserId == CurrentUserId)
        //         ?? throw new NotFoundException("Payment not found.");

        //     payment.UpdatePayment(request.Amount, request.PaymentMethod, request.Note);
        //     await Context.SaveChangesAsync();

        //     return Ok(new PaymentResponse(payment));
        // }

        // DELETE: api/payments/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment([FromRoute] string id)
        {
            var payment = await Context.Payments
                .Include(p => p.Debt)
                .ThenInclude(d => d.Debtor)
                .FirstOrDefaultAsync(p => p.Id == id && p.Debt.Debtor.UserId == CurrentUserId)
                ?? throw new NotFoundException("Payment not found.");

            Context.Payments.Remove(payment);
            await Context.SaveChangesAsync();

            return NoContent();
        }
    }
}
