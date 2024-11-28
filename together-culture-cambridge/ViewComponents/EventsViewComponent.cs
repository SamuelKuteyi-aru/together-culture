using Microsoft.AspNetCore.Mvc;

namespace together_culture_cambridge.ViewComponents;

public class EventsViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}