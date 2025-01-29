using WebApi.Models;

namespace WebApi.DTOs;

public class CreateProductRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public required string CategoryId { get; set; }
}


public class ProductResponse(Product product)
{
    public string Id { get; set; } = product.Id;
    public string Name { get; set; } = product.Name;
    public string? Description { get; set; } = product.Description;
    public decimal UnitPrice { get; set; } = product.UnitPrice;
    public int StockQuantity { get; set; } = product.StockQuantity;
    public DateTime CreatedAt { get; set; } = product.CreatedAt;
    public DateTime UpdatedAt { get; set; } = product.UpdatedAt;
    public string CategoryName { get; set; } = product.Category.Name;
}

