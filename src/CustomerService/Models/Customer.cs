namespace NorthwindTraders.CustomerService.Models;

public class Customer
{
    public string CustomerId { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string? ContactName { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
