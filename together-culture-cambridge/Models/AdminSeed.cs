using Microsoft.EntityFrameworkCore;
using together_culture_cambridge.Data;
namespace together_culture_cambridge.Models;

public class AdminSeed
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context =
               new ApplicationDatabaseContext(serviceProvider
                   .GetRequiredService<DbContextOptions<ApplicationDatabaseContext>>()))
        {
            if (context.Admin.Any()) return;
            context.Admin.AddRange(
                new Admin
                {
                    FirstName = "Samuel",
                    LastName = "Kuteyi",
                    Phone = "+447444104253",
                    Email = "kuteyisamueldev@gmail.com",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("", 13),
                    CreatedAt = DateTime.Now,
                }
            );
            
            context.SaveChanges();
        }
    }
}