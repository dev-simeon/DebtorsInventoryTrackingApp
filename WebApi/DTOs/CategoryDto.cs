using WebApi.Models;

namespace WebApi.DTOs
{
    public class CategoryCreateRequest
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
    }

    public class CategoryResponse(Category category)
    {
        public string Id { get; } = category.Id;
        public string Name { get; } = category.Name;
        public string? Description { get; } = category.Description;
    }
}