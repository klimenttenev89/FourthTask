namespace NorthwindTraders.CustomerService.Models;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal? UnitPrice { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
