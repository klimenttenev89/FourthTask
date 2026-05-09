using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Frontend.Services;

namespace NorthwindTraders.Frontend.Controllers;

public class CustomersController : Controller
{
    private readonly ApiService _api;

    public CustomersController(ApiService api) => _api = api;

    private bool IsAuthenticated => HttpContext.Session.GetString("jwt") is not null;

    public async Task<IActionResult> Index(string? search)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");

        var customers = await _api.GetCustomersAsync(search);
        ViewBag.Search = search;
        return View(customers);
    }

    public async Task<IActionResult> Details(string id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");

        var customer = await _api.GetCustomerAsync(id);
        if (customer is null) return NotFound();
        return View(customer);
    }
}
