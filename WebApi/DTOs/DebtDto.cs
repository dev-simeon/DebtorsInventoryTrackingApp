using WebApi.Models;

namespace WebApi.DTOs;

public class CreateDebtRequest
{
    public required decimal TotalAmount { get; init; }
    public required DateTime DueDate { get; init; }
    public decimal? AmountOwed { get; init; }
}

public class ExtendDebtDueDateRequest
{
    public required int ExtendDays { get; init; }
}


public class DebtResponse(Debt debt)
{
    public string DebtId { get; } = debt.Id;
    public string Debtor { get; } = debt.Debtor.FullName;
    public decimal TotalDebt { get; } = debt.TotalDebt;
    public decimal AmountOwed { get; } = debt.AmountOwed;
    public string Status { get; } = debt.Status;
    public DateTime DueDate { get; } = debt.DueDate;
}
