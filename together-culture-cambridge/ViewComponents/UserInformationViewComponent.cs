using Microsoft.AspNetCore.Mvc;

namespace together_culture_cambridge.ViewComponents;

public class UserInformationViewComponent : ViewComponent
{
   public IViewComponentResult Invoke()
   {
      return View();
   }
}