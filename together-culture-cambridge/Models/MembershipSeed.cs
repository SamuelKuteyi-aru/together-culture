using Microsoft.EntityFrameworkCore;
using together_culture_cambridge.Data;

namespace together_culture_cambridge.Models;

public class MembershipSeed
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDatabaseContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDatabaseContext>>()))
        {

            if (context.Membership.Any())
            {
               // System.Console.WriteLine("There are missing membership records.");
                return;
            }
            
            //System.Console.WriteLine("Seeding data");
            context.Membership.AddRange(
                new Membership
                {
                    MonthlyPrice = 0,
                    JoiningFee = 0,
                    MembershipType = Membership.MembershipEnum.Guest,
                    Name= "Guest"
                },
                new Membership
                {
                    MonthlyPrice = 18.50,
                    JoiningFee = 0,
                    MembershipType = Membership.MembershipEnum.Community,
                    Name= "Community"
                },
                new Membership
                {
                    MonthlyPrice = 45,
                    JoiningFee = 70,
                    MembershipType = Membership.MembershipEnum.KeyAccess,
                    Name= "Key Access"
                },
                new Membership
                {
                    MonthlyPrice = 70,
                    JoiningFee = 100,
                    MembershipType = Membership.MembershipEnum.CreativeWorkspace,
                    Name= "Creative Workspace"
                }
            );
            context.SaveChanges();
        }
    }
}