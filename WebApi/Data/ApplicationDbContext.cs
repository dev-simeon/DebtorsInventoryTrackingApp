using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Debtor> Debtors { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User-Debtor relationship (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Debtors)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Products)
                .WithOne(p => p.Owner)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Debtor-Debt relationship (One-to-Many)
            modelBuilder.Entity<Debtor>()
                .HasMany(d => d.Debts)
                .WithOne(debt => debt.Debtor)
                .HasForeignKey(debt => debt.DebtorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Debt-Payment relationship (One-to-Many)
            modelBuilder.Entity<Debt>()
                .HasMany(debt => debt.Payments)
                .WithOne(payment => payment.Debt)
                .HasForeignKey(payment => payment.DebtId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Address as owned entity
            modelBuilder.Entity<Debtor>()
                .OwnsOne(d => d.Address, address =>
                {
                    address.Property(a => a.Street).IsRequired();
                    address.Property(a => a.City).IsRequired();
                    address.Property(a => a.State).IsRequired();
                    address.Property(a => a.ZipCode).IsRequired(false);
                });


            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // Configure Product-InventoryTransaction relationship (One-to-Many)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.StockMovements)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // This will delete associated inventory transactions if product is deleted.
        }
    }
}
