using WebApi.Models;

namespace WebApi.DTOs;

public class CreatePaymentRequest
{
    public required string DebtId { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
}

public class PaymentResponse(Payment payment)
{
    public string Id { get; set; } = payment.Id;
    public string DebtId { get; set; } = payment.DebtId;
    public decimal TotalAmountOwed { get; set; } = payment.Debt.AmountOwed;
    public decimal AmountPaid { get; set; } = payment.Amount;
    public string PaymentMethod { get; set; } = payment.PaymentMethod;
    public string? Note { get; set; } = payment.Note;
    public DateTime PaymentDate { get; set; } = payment.PaymentDate;
}
