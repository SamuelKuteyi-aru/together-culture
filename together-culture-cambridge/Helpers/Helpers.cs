using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Relational;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using together_culture_cambridge.Data;
using together_culture_cambridge.Models;
using Row = Mysqlx.Resultset.Row;

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

    public static int ReadUserCookie(HttpRequest request)
    {
        var userCookie = request.Cookies["tc-session-user"];
        if (String.IsNullOrEmpty(userCookie))
        {
            return -1;
        }
            
        byte[] byteArray = Convert.FromBase64String(userCookie);
        string base64String = System.Text.Encoding.UTF8.GetString(byteArray);

        return int.Parse(base64String);
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

    public static JsonResult CreateEventItem(Event @event, ApplicationDatabaseContext context)
    {
        var bookingList = context.EventBooking.Where(x => x.EventId == @event.Id).ToList();
        return new JsonResult(new
        {
            id = @event.Id,
            name = @event.Name,
            address = @event.Address,
            description = @event.Description,
            totalSpaces = @event.TotalSpaces,
            ticketPrice = @event.TicketPrice,
            startTime = @event.StartTime,
            endTime = @event.EndTime,
            createdAt = @event.CreatedAt,
            bookedSpaces = bookingList.Count,
        });
    }

    public static JsonArray CreateEventList(List<Event> events, ApplicationDatabaseContext context)
    {
        JsonArray eventArray = new JsonArray();
        foreach (var @event in events)
        {
            var result = CreateEventItem(@event, context);
            eventArray.Add(result.Value);
        }

        return eventArray;
    } 



    public static List<SpaceBooking> GetSpaceBookings(Space spaceItem, ApplicationDatabaseContext context)
    {
        var bookings = context.SpaceBooking.Where(
            booking =>
                booking.SpaceId == spaceItem.Id
        ).Join(context.Space,
            booking => booking.SpaceId,
            space => space.Id,
            (booking, space) => new { booking, space }
        ).Join(context.EndUser, 
            bookingData => bookingData.booking.EndUserId,
            user => user.Id,
            (bookingData, user) => new SpaceBooking
            {
                EndUser = user,
                EndUserId = user.Id,
                Space = bookingData.space,
                SpaceId = bookingData.space.Id,
                BookingDate = bookingData.booking.BookingDate,
            }
        ).Join(context.Membership,
            spaceBooking => spaceBooking.EndUser.MembershipId,
            membership => membership.Id, 
            (spaceBooking, membership) => new { spaceBooking, membership }
        ).ToList();


        List<SpaceBooking> bookingList = [];
        foreach (var booking in bookings)
        {
            if ((DateTime.Now - booking.spaceBooking.BookingDate).Days == 0)
            {
                booking.spaceBooking.EndUser.Membership = booking.membership;
                bookingList.Add(booking.spaceBooking);
            }
        }

        return bookingList;
    }


    public static JsonResult CreateSpaceItem(Space spaceItem, ApplicationDatabaseContext context)
    {
       var spaceBookings = GetSpaceBookings(spaceItem, context);
       return new JsonResult(new
       {
           id = spaceItem.Id,
           minimumAccessLevel = spaceItem.MinimumAccessLevel,
           totalSeats = spaceItem.TotalSeats,
           openingTime = spaceItem.OpeningTime,
           closingTime = spaceItem.ClosingTime,
           roomId = spaceItem.RoomId,
           createdAt = spaceItem.CreatedAt,
           bookedSeats = spaceBookings.Count
       });
    }

    public static JsonArray CreateSpaceList(List<Space> spaces, ApplicationDatabaseContext context)
    {
        JsonArray spaceArray = new JsonArray();
        foreach (var space in spaces)
        {
            var result = CreateSpaceItem(space, context);
            spaceArray.Add(result.Value);
        }
        
        return spaceArray;
    }
}