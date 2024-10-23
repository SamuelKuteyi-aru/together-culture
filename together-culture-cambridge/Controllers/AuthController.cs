using Microsoft.AspNetCore.Mvc;

namespace together_culture_cambridge.Controllers;

public class AuthController : Controller
{
    // GET
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }
}