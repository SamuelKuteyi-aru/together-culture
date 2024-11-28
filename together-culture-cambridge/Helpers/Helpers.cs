using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using together_culture_cambridge.Models;

namespace together_culture_cambridge.Helpers;

public class Methods
{
    public static int GenerateCode(int length = 6)
    {
        Random random = new Random();
        return random.Next(100000, 999999);
    }

    public static JsonResult PublicFacingAdmin(Admin admin)
    {
        return new JsonResult(new
        {
            id = admin.Id,
            firstName = admin.FirstName,
            lastName = admin.LastName,
            email = admin.Email,
            phone = admin.Phone,
            createdAt = admin.CreatedAt,
        });
    }

    public static JsonResult PublicFacingUser(EndUser endUser)
    {
        EndUser.GenderEnum genderValue = endUser.Gender;
        var membership = endUser.Membership;
        Console.WriteLine("Membership: {0}", membership);
        object? membershipData = null;
        if (membership != null)
        {
            var membershipValue = membership.MembershipType;

            membershipData = new JsonResult(new
            {
                name = membership.Name,
                membershipType = membershipValue,
                joiningFee = membership.JoiningFee,
                monthlyPrice = membership.MonthlyPrice,
            }).Value;
        }

        

        return new JsonResult(new
        {
            id = endUser.Id,
            firstName = endUser.FirstName,
            lastName = endUser.LastName,
            email = endUser.Email,
            createdAt = endUser.CreatedAt,
            checkIn = endUser.CheckIn,
            checkOut = endUser.CheckOut,
            dateOfBirth = endUser.DateOfBirth,
            membership = membershipData,
            membershipId = endUser.MembershipId,
            phone = endUser.Phone,
            subscriptionDate = endUser.SubscriptionDate,
            gender = genderValue.ToString(),
            approved = endUser.Approved,
        });
    }

    public static int ReadAdminCookie(HttpRequest request)
    {
        var adminCookie = request.Cookies["tc-session-admin"];
        if (String.IsNullOrEmpty(adminCookie))
        {
            return -1;
        }
            
        byte[] byteArray = Convert.FromBase64String(adminCookie);
        string base64String = System.Text.Encoding.UTF8.GetString(byteArray);

        return int.Parse(base64String);
    }

    public static CookieOptions CreateCookieOptions()
    {
        CookieOptions cookieOptions = new CookieOptions();
        cookieOptions.Expires = DateTime.Now.AddDays(1);
        cookieOptions.Secure = true;
        cookieOptions.HttpOnly = true;
        cookieOptions.SameSite = SameSiteMode.Strict;
        cookieOptions.MaxAge = TimeSpan.FromDays(1);

        return cookieOptions;
    }
    
    public  static bool FilterUser(
        EndUser user, 
        bool passedUnapprovedQuery, 
        bool unapproved,
        string query
    )
    {
        if (user.Email != null && user.Email.Contains(query))
        {
            if (passedUnapprovedQuery) return user.Approved == !unapproved;
            return true;
        }
                
        var firstName = String.IsNullOrEmpty(user.FirstName) ? "" : user.FirstName;
        var lastName = String.IsNullOrEmpty(user.LastName) ? "" : user.LastName;
        var userName = firstName + lastName;

        if (userName.Contains(query))
        {
            if (passedUnapprovedQuery) return user.Approved == !unapproved;
            return true;
        }


        return false;
    }
}