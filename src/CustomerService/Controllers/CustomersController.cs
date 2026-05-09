using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.CustomerService.Repositories;

namespace NorthwindTraders.CustomerService.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repo;

    public CustomersController(ICustomerRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search)
    {
        var customers = await _repo.GetCustomersAsync(search);
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("Customer ID is required.");

        var customer = await _repo.GetCustomerByIdAsync(id.ToUpperInvariant());
        return customer is null ? NotFound() : Ok(customer);
    }
}
