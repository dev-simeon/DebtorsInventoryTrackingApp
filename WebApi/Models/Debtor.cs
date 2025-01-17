using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Debtor
    {
        public Debtor(string fullName, string phone, string email, Address address)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fullName);

            Id = CreateUniqueDebtorId(fullName);
            FullName = fullName;
            UpdateContactInfo(phone, email);
            UpdateAddress(address);
            CalculateTotalDebt();
            CreatedAt = DateTime.UtcNow;
        }

        private static string CreateUniqueDebtorId(string fullName)
        {
            var identifier = fullName.Length >= 4 ? fullName[..4] : fullName.PadRight(4, 'X');
            return $"{identifier}_{Guid.NewGuid():N}".ToUpper();
        }

        public void CalculateTotalDebt()
        {
            OutstandingDebt = Debts?.Sum(debt => debt.AmountOwed) ?? 0;
        }

        public void UpdateLastPaymentDate(DateTime paymentDate)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(paymentDate, DateTime.UtcNow);
            LastPaymentDate = paymentDate;
        }

        public void AddDebt(Debt debt)
        {
            ArgumentNullException.ThrowIfNull(debt);
            Debts.Add(debt);
            CalculateTotalDebt();
        }

        public void RemoveDebt(Debt debt)
        {
            ArgumentNullException.ThrowIfNull(debt);
            Debts.Remove(debt);
            CalculateTotalDebt();
        }

        public void UpdateContactInfo(string newPhone, string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newPhone) || !new PhoneAttribute().IsValid(newPhone))
                throw new ArgumentException("A valid phone number is required.", nameof(newPhone));

            if (string.IsNullOrWhiteSpace(newEmail) || !new EmailAddressAttribute().IsValid(newEmail))
                throw new ArgumentException("A valid email address is required.", nameof(newEmail));

            Phone = newPhone;
            Email = newEmail;
        }

        public void UpdateAddress(Address newAddress)
        {
            ArgumentNullException.ThrowIfNull(newAddress, nameof(newAddress));
            Address = newAddress;
        }


        [Key]
        public string Id { get; private set; }
        public string UserId { get; private set; } = string.Empty;
        public string FullName { get; private set; }
        public string Phone { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public Address Address { get; private set; } = default!;
        [Column(TypeName = "decimal(18, 2)")]
        public decimal OutstandingDebt { get; private set; }
        public DateTime? LastPaymentDate { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public User User { get; private set; } = default!;
        public virtual ICollection<Debt> Debts { get; private set; } = [];

#pragma warning disable CS8618
        private Debtor() { }
    }


    [Owned]
    public class Address
    {
        public Address(string street, string city, string state, string? zipCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(street);
            ArgumentException.ThrowIfNullOrWhiteSpace(city);
            ArgumentException.ThrowIfNullOrWhiteSpace(state);

            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
        }

        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string? ZipCode { get; private set; }
    }
}