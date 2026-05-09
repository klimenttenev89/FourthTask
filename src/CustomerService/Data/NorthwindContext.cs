using Microsoft.EntityFrameworkCore;
using NorthwindTraders.CustomerService.Models;

namespace NorthwindTraders.CustomerService.Data;

public class NorthwindContext : DbContext
{
    public NorthwindContext(DbContextOptions<NorthwindContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.CustomerId).HasMaxLength(5);
            entity.Property(e => e.CompanyName).HasMaxLength(40).IsRequired();
            entity.Property(e => e.ContactName).HasMaxLength(30);
            entity.Property(e => e.City).HasMaxLength(15);
            entity.Property(e => e.Country).HasMaxLength(15);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.Orders)
                  .HasForeignKey(e => e.CustomerId);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId });
            entity.ToTable("OrderDetails");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.Order)
                  .WithMany(o => o.OrderDetails)
                  .HasForeignKey(e => e.OrderId);
            entity.HasOne(e => e.Product)
                  .WithMany(p => p.OrderDetails)
                  .HasForeignKey(e => e.ProductId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.ProductName).HasMaxLength(40).IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
        });
    }
}
