using Microsoft.EntityFrameworkCore;
using together_culture_cambridge.Data;

namespace together_culture_cambridge.Models;


public class UserSeed
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDatabaseContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDatabaseContext>>()))
        {
            if (context.EndUser.Any()) return;
            context.EndUser.AddRange(
                new EndUser
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@gmail.com",
                    Password = "123456",
                    Gender = EndUser.GenderEnum.Male,
                    DateOfBirth = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                }
            );
            context.SaveChanges();
        }
    }
}