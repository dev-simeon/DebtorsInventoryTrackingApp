using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class User
    {
        public User(string firstName, string lastName, string email, string password)
        {
            Id = email;
            UpdatePassword(null, password);
            UpdateProfile(firstName, lastName, email);
            CreatedAt = DateTime.UtcNow;
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }

        public void UpdatePassword(string? oldPassword, string newPassword)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(newPassword);

            // Allow setting password without oldPassword validation if it's the first time (PasswordHash is null/empty).
            if (!string.IsNullOrEmpty(PasswordHash) && (string.IsNullOrWhiteSpace(oldPassword) || !VerifyPassword(oldPassword)))
                throw new ArgumentException("Incorrect password provided", nameof(oldPassword));

            if (newPassword.Length < 8 || !newPassword.Any(char.IsUpper) || !newPassword.Any(char.IsDigit))
                throw new ArgumentException("Password must be at least 8 characters long and include an uppercase letter and a number.");

            PasswordHash = HashPassword(newPassword);
        }

        public void UpdateProfile(string firstName, string lastName, string email)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);

            if (!new EmailAddressAttribute().IsValid(email))
                throw new ArgumentException("A valid Email is required.", nameof(email));

            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        [Key]
        public string Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        public virtual ICollection<Debtor> Debtors { get; private set; } = [];
        public virtual ICollection<Product> Products { get; private set; } = [];

#pragma warning disable CS8618
        private User() { }
    }
}
