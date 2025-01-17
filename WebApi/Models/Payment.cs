using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class Payment
    {
        public Payment(decimal amount, string paymentMethod, string? note)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            ArgumentException.ThrowIfNullOrWhiteSpace(paymentMethod);

            Id = Guid.NewGuid().ToString();
            Amount = amount;
            PaymentMethod = ValidatePaymentMethod(paymentMethod);
            Note = note;
            PaymentDate = DateTime.UtcNow;
        }

        private static string ValidatePaymentMethod(string paymentMethod)
        {
            // Example: Validate against a list of allowed payment methods
            var allowedMethods = new[] { "Cash", "Card", "Bank Transfer" };

            if (!allowedMethods.Contains(paymentMethod))
                throw new ArgumentException($"Invalid payment method. Allowed methods are: {string.Join(", ", allowedMethods)}", nameof(paymentMethod));

            return paymentMethod;
        }

        public void UpdatePayment(decimal amount, string paymentMethod, string? note)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
            // ArgumentOutOfRangeException.ThrowIfGreaterThan(amount, Debt.AmountOwed);
            ArgumentException.ThrowIfNullOrWhiteSpace(paymentMethod);

            Id = Guid.NewGuid().ToString();
            Amount = amount;
            PaymentMethod = ValidatePaymentMethod(paymentMethod);
            Note = note;
            PaymentDate = DateTime.UtcNow; // Update payment date when modified
        }

        [Key]
        public string Id { get; private set; }
        public string DebtId { get; private set; } = string.Empty;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public string? Note { get; private set; }
        public string PaymentMethod { get; private set; }

        public Debt Debt { get; private set; } = default!;

#pragma warning disable CS8618
        private Payment() { } // Parameterless constructor for EF
    }
}
