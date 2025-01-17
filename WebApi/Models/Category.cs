namespace WebApi.Models
{
    public class Category
    {
        public Category(string name, string? description)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            Id = name;
            Name = name;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateCategory(string name, string? description = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            Name = name;
            Description = description;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? Description { get; private set; }

        public virtual ICollection<Product> Products { get; private set; } = [];


#pragma warning disable CS8618 
        private Category() { }
    }
}