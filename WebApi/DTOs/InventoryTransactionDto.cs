using WebApi.Models;

namespace WebApi.DTOs;


public class InventoryTransactionRequest
{
    public required string ProductId { get; init; }
    public required int Quantity { get; init; }
    public string? Notes { get; init; }
}

public class InventoryTransactionResponse(InventoryTransaction transaction)
{
    public string Id { get; set; } = transaction.Id;
    public string ProductId { get; set; } = transaction.ProductId;
    public string ProductName { get; set; } = transaction.Product.Name;
    public int Quantity { get; set; } = transaction.Quantity;
    public string TransactionType { get; set; } = transaction.TransactionType;
    public DateTime TransactionDate { get; set; } = transaction.TransactionDate;
    public string? Notes { get; set; } = transaction.Notes;
}

