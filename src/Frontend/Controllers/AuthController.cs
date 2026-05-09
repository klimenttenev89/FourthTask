using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Frontend.Models;
using NorthwindTraders.Frontend.Services;

namespace NorthwindTraders.Frontend.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _api;

    public AuthController(ApiService api) => _api = api;

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var token = await _api.LoginAsync(model.Username, model.Password);
        if (token is null)
        {
            model.ErrorMessage = "Invalid username or password.";
            return View(model);
        }

        HttpContext.Session.SetString("jwt", token);
        return RedirectToAction("Index", "Customers");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
