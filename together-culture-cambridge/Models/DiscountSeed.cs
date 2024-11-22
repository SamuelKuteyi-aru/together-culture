using Microsoft.EntityFrameworkCore;
using together_culture_cambridge.Data;

namespace together_culture_cambridge.Models;

public class DiscountSeed
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context =
               new ApplicationDatabaseContext(serviceProvider
                   .GetRequiredService<DbContextOptions<ApplicationDatabaseContext>>()))
        {
            if (context.Discount.Any())
            {
                return;
            }


            context.Discount.AddRange(
                new Discount
                {
                    Code = "MEMBERSHIP20",
                    Percentage = .2,
                    CreatedAt = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddMonths(6)
                },
                new Discount
                {
                    Code = "JOIN15",
                    Percentage = .15,
                    CreatedAt = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddMonths(6)
                },
                new Discount
                {
                    Code="TC25",
                    Percentage = .25,
                    CreatedAt = DateTime.Now,
                    ExpirationDate = DateTime.Now.AddMonths(6)
                }
            );
            
            context.SaveChanges();
        }
    }
}
