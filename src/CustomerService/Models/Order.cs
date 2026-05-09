namespace NorthwindTraders.CustomerService.Models;

public class Order
{
    public int OrderId { get; set; }
    public string? CustomerId { get; set; }
    public DateTime? OrderDate { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
