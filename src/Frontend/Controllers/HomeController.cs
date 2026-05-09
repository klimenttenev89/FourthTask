using Microsoft.AspNetCore.Mvc;

namespace NorthwindTraders.Frontend.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("jwt") is not null)
            return RedirectToAction("Index", "Customers");
        return RedirectToAction("Login", "Auth");
    }
}
