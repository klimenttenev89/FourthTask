using NorthwindTraders.CustomerService.DTOs;

namespace NorthwindTraders.CustomerService.Repositories;

public interface ICustomerRepository
{
    Task<IEnumerable<CustomerListDto>> GetCustomersAsync(string? search);
    Task<CustomerDetailDto?> GetCustomerByIdAsync(string id);
}
