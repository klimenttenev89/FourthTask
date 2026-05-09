using Microsoft.EntityFrameworkCore;
using NorthwindTraders.CustomerService.Data;
using NorthwindTraders.CustomerService.Models;
using NorthwindTraders.CustomerService.Repositories;
using Xunit;

namespace CustomerService.Tests;

public class CustomerRepositoryTests
{
    private static NorthwindContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<NorthwindContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new NorthwindContext(options);
    }

    private static async Task<NorthwindContext> SeedContextAsync()
    {
        var ctx = CreateContext();

        ctx.Products.AddRange(
            new Product { ProductId = 1, ProductName = "Chai", UnitPrice = 18m },
            new Product { ProductId = 2, ProductName = "Chang", UnitPrice = 19m });

        ctx.Customers.AddRange(
            new Customer { CustomerId = "ALFKI", CompanyName = "Alfreds Futterkiste", ContactName = "Maria Anders", Country = "Germany" },
            new Customer { CustomerId = "ANATR", CompanyName = "Ana Trujillo", ContactName = "Ana Trujillo", Country = "Mexico" });

        ctx.Orders.AddRange(
            new Order { OrderId = 1, CustomerId = "ALFKI", OrderDate = new DateTime(2024, 1, 15) },
            new Order { OrderId = 2, CustomerId = "ALFKI", OrderDate = new DateTime(2024, 3, 22) },
            new Order { OrderId = 3, CustomerId = "ANATR", OrderDate = new DateTime(2024, 2, 10) });

        ctx.OrderDetails.AddRange(
            new OrderDetail { OrderId = 1, ProductId = 1, UnitPrice = 18m, Quantity = 10, Discount = 0 },
            new OrderDetail { OrderId = 1, ProductId = 2, UnitPrice = 19m, Quantity = 5, Discount = 0 },
            new OrderDetail { OrderId = 2, ProductId = 1, UnitPrice = 18m, Quantity = 3, Discount = 0.05f },
            new OrderDetail { OrderId = 3, ProductId = 2, UnitPrice = 19m, Quantity = 4, Discount = 0 });

        await ctx.SaveChangesAsync();
        return ctx;
    }

    [Fact]
    public async Task GetCustomersAsync_ReturnsAllCustomers_WhenSearchIsNull()
    {
        await using var ctx = await SeedContextAsync();
        var repo = new CustomerRepository(ctx);

        var result = (await repo.GetCustomersAsync(null)).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetCustomersAsync_FiltersCustomers_ByCompanyName()
    {
        await using var ctx = await SeedContextAsync();
        var repo = new CustomerRepository(ctx);

        var result = (await repo.GetCustomersAsync("Alfreds")).ToList();

        Assert.Single(result);
        Assert.Equal("ALFKI", result[0].CustomerId);
    }

    [Fact]
    public async Task GetCustomersAsync_FiltersCustomers_ByContactName()
    {
        await using var ctx = await SeedContextAsync();
        var repo = new CustomerRepository(ctx);

        var result = (await repo.GetCustomersAsync("Ana")).ToList();

        Assert.Single(result);
        Assert.Equal("ANATR", result[0].CustomerId);
    }

    [Fact]
    public async Task GetCustomersAsync_ReturnsCorrectOrderCount()
    {
        await using var ctx = await SeedContextAsync();
        var repo = new CustomerRepository(ctx);

        var result = (await repo.GetCustomersAsync(null)).ToList();
        var alfki = result.Single(c => c.CustomerId == "ALFKI");

        Assert.Equal(2, alfki.OrderCount);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_ReturnsCustomerWithOrders()
    {
        await using var ctx = await SeedContextAsync();
        var repo = new CustomerRepository(ctx);

        var detail = await repo.GetCustomerByIdAsync("ALFKI");

        Assert.NotNull(detail);
        Assert.Equal("ALFKI", detail.CustomerId);
        Assert.Equal(2, detail.Orders.Count());
    }

    [Fact]
    public async Task GetCustomerByIdAsync_ReturnsNull_ForUnknownId()
    {
        await using var ctx = await SeedContextAsync();
        var repo = new CustomerRepository(ctx);

        var detail = await repo.GetCustomerByIdAsync("XXXXX");

        Assert.Null(detail);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_CalculatesOrderTotalCorrectly()
    {
        await using var ctx = await SeedContextAsync();
        var repo = new CustomerRepository(ctx);

        var detail = await repo.GetCustomerByIdAsync("ALFKI");
        var order1 = detail!.Orders.Single(o => o.OrderId == 1);

        // Order 1: (18 * 10 * 1) + (19 * 5 * 1) = 180 + 95 = 275
        Assert.Equal(275m, order1.TotalValue);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_CountsProductsPerOrder()
    {
        await using var ctx = await SeedContextAsync();
        var repo = new CustomerRepository(ctx);

        var detail = await repo.GetCustomerByIdAsync("ALFKI");
        var order1 = detail!.Orders.Single(o => o.OrderId == 1);

        Assert.Equal(2, order1.ProductCount);
    }

    [Fact]
    public async Task GetCustomersAsync_ReturnsEmpty_WhenNoMatch()
    {
        await using var ctx = await SeedContextAsync();
        var repo = new CustomerRepository(ctx);

        var result = (await repo.GetCustomersAsync("ZZZ_NO_MATCH_ZZZ")).ToList();

        Assert.Empty(result);
    }
}
