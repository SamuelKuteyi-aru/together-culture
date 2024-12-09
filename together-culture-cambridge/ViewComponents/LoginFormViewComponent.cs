using Microsoft.AspNetCore.Mvc;

namespace together_culture_cambridge.ViewComponents;

public class LoginFormViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}