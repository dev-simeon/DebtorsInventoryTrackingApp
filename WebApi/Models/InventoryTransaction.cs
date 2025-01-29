namespace WebApi.Models
{
    public class InventoryTransaction
    {
        public InventoryTransaction(int quantity, string transactionType, string? notes)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

            Id = Guid.NewGuid().ToString();
            Quantity = quantity;
            TransactionType = transactionType;
            Notes = notes;
        }

        public string Id { get; private set; }
        public string ProductId { get; private set; } = string.Empty;
        public int Quantity { get; private set; }
        public string TransactionType { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public string? Notes { get; private set; }

        public Product Product { get; set; } = default!;

#pragma warning disable CS8618
        private InventoryTransaction() { }
    }
}