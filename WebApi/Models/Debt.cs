using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class Debt
    {
        public Debt(decimal totalAmount, DateTime dueDate, decimal? amountOwed = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(totalAmount);
            ArgumentOutOfRangeException.ThrowIfLessThan(DueDate, DateTime.UtcNow);

            if (amountOwed.HasValue)
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amountOwed.Value, nameof(amountOwed));

            Id = Guid.NewGuid().ToString();
            TotalDebt = totalAmount;
            DueDate = dueDate;
            AmountOwed = amountOwed ?? totalAmount;
            RecalculateAmountOwed();
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus()
        {
            Status = AmountOwed switch
            {
                _ when IsOverdue() => "Overdue",
                var owed when owed == TotalDebt => "Pending Payment",
                0 => "Paid",
                _ => "Partially Paid"
            };
        }

        public void RecordPayment(decimal amount, string? note = null, string paymentMethod = "Cash")
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(amount, AmountOwed);

            Payments.Add(new Payment(amount, paymentMethod, note));
            AmountOwed -= amount;
            RecalculateAmountOwed();
        }

        public void ExtendDueDate(int days)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(days);
            DueDate = DueDate.AddDays(days);
            RecalculateAmountOwed();
        }

        public void RecalculateAmountOwed()
        {
            AmountOwed = TotalDebt - Payments.Sum(payment => payment.Amount);
            UpdateStatus();
        }

        public bool IsOverdue() => DateTime.UtcNow > DueDate && AmountOwed > 0;


        [Key]
        public string Id { get; private set; }
        public string DebtorId { get; private set; } = string.Empty;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalDebt { get; private set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal AmountOwed { get; private set; }
        public DateTime DueDate { get; private set; }
        public string Status { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        // Navigation properties
        public Debtor Debtor { get; private set; } = default!;
        public virtual ICollection<Payment> Payments { get; private set; } = [];

#pragma warning disable CS8618
        private Debt() { }
    }
}
