using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models
{
    public class Product
    {
        public Product(string name, string? description, decimal unitPrice, int stockQuantity)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(unitPrice);
            ArgumentOutOfRangeException.ThrowIfNegative(stockQuantity);

            Id = GenerateUniqueId();
            Name = name;
            Description = description;
            UnitPrice = unitPrice;
            StockQuantity = stockQuantity;
            CreatedAt = DateTime.UtcNow;
        }

        private string GenerateUniqueId()
        {
            return $"{Category.Name}_{Name}".Replace(" ", "_").ToLower();
        }


        public string Id { get; private set; }
        public string OwnerId { get; private set; } = string.Empty;
        public string CategoryId { get; private set; } = string.Empty;
        public string Name { get; private set; }
        public string? Description { get; private set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; private set; }
        public int StockQuantity { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public User Owner { get; private set; } = default!;
        public Category Category { get; private set; } = default!;
        public ICollection<InventoryTransaction> StockMovements { get; private set; } = [];

#pragma warning disable CS8618
        private Product() { }
    }
}
