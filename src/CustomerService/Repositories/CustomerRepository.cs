using Microsoft.EntityFrameworkCore;
using NorthwindTraders.CustomerService.Data;
using NorthwindTraders.CustomerService.DTOs;

namespace NorthwindTraders.CustomerService.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly NorthwindContext _context;

    public CustomerRepository(NorthwindContext context) => _context = context;

    public async Task<IEnumerable<CustomerListDto>> GetCustomersAsync(string? search)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c =>
                c.CompanyName.Contains(term) ||
                (c.ContactName != null && c.ContactName.Contains(term)));
        }

        return await query
            .OrderBy(c => c.CompanyName)
            .Select(c => new CustomerListDto(
                c.CustomerId,
                c.CompanyName,
                c.ContactName,
                c.Country,
                c.Orders.Count))
            .ToListAsync();
    }

    public async Task<CustomerDetailDto?> GetCustomerByIdAsync(string id)
    {
        var customer = await _context.Customers
            .Include(c => c.Orders)
                .ThenInclude(o => o.OrderDetails)
            .FirstOrDefaultAsync(c => c.CustomerId == id);

        if (customer is null) return null;

        var orders = customer.Orders.Select(o => new OrderSummaryDto(
            o.OrderId,
            o.OrderDate,
            o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount)),
            o.OrderDetails.Count));

        return new CustomerDetailDto(
            customer.CustomerId,
            customer.CompanyName,
            customer.ContactName,
            customer.City,
            customer.Country,
            orders);
    }
}
