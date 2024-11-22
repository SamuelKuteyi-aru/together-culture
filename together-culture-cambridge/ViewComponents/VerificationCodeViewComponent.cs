using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace together_culture_cambridge.ViewComponents;

public class VerificationCodeViewComponent :ViewComponent
{
   public IViewComponentResult Invoke()
   {
      return View();
   }
}