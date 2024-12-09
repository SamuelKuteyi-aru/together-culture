using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using together_culture_cambridge.Data;
using together_culture_cambridge.Models;
using together_culture_cambridge.Helpers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;


namespace together_culture_cambridge.Controllers
{
    public class EndUserController : Controller
    {
        private readonly ApplicationDatabaseContext _context;

        public EndUserController(ApplicationDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/EndUser")]

        public async Task<IActionResult> Get()
        {
            int userId = Methods.ReadUserCookie(Request);

            if (userId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }

            var userData = await _context.EndUser
                .Where(endUser => endUser.Id == userId)
                .Join(_context.Membership,
                    user => user.MembershipId,
                    membership => membership.Id,
                    (user, membership) => new { membership, endUser = user }
                 ).FirstOrDefaultAsync();
            if (userData == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "User not found" });
            }

            

            userData.endUser.Membership = userData.membership;
            var user = userData.endUser;
            if (!user.Approved)
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                return Json(new { message = "This account is not yet approved" });
            }

            var publicUser = Methods.PublicFacingUser(user).Value;

            var eventBookingList = Methods.GetBookedEvents(_context, user.Id);
            var eventBookings = Methods.CreateEventList(eventBookingList, _context);
            
            var spaceBookingList = Methods.GetBookedSpaces(_context, user.Id);
            var spaceBookings = Methods.CreateBookedSpaceList(spaceBookingList, _context, user.Id);

            var result = new JsonResult(new
            {
                user = publicUser,
                spaceBookings = spaceBookings,
                eventBookings = eventBookings,
            });
            return Ok(result.Value);
        }

        // Route: /EndUser/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // Route: /EndUser/Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }
        
        // Route: /EndUser/Events
        public IActionResult Events()
        {
            return View();
        }
        
        // Route: /EndUser/Spaces
        public IActionResult Spaces()
        {
            return View();
        }
        
        public IActionResult LogOut()
        {
            if (Request.Cookies["tc-session-user"] != null)
            {
                Response.Cookies.Delete("tc-session-user");
            }
            
            return RedirectToAction("Login", "EndUser");
        }
        
        // Route: /EndUser/Unapproved
        [HttpGet]

        public async Task<IActionResult> Unapproved()
        {
            int adminId = Methods.ReadAdminCookie(Request);
            if (adminId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }

            var guestMembership = await _context.Membership
                .Where(m => m.MembershipType == Enum.Parse<Membership.MembershipEnum>("Guest")).FirstOrDefaultAsync();
          //  Console.WriteLine("Guest membership: {0}", guestMembership?.Id);
            var userList = await _context.EndUser
                .Where(user => !user.Approved && (guestMembership != null && user.MembershipId != guestMembership.Id))
                .Join(
                    _context.Membership,
                    endUser => endUser.MembershipId,
                    membership => membership.Id,
                    (endUser, membership) => 
                        new {endUser, membership}
                )
                .ToListAsync();

            JsonArray users = new JsonArray();
            foreach (var user in userList)
            {

                user.endUser.Membership = user.membership;
                users.Add(Methods.PublicFacingUser(user.endUser).Value);
            }

            return Ok(users);

        }

        [Route("EndUser/Search")]
        public async Task<IActionResult> AccountSearch()
        {
            int adminId = Methods.ReadAdminCookie(Request);
            if (adminId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }

            if (!Request.Query.Keys.Contains("query"))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Query parameter is required" });
            }

            var query = Request.Query["query"].ToString().ToLower();

            bool passedApprovedQuery = Request.Query.Keys.Contains("approved");
            bool approved = true;
            if (passedApprovedQuery)
            {
                var approvedQuery = Request.Query["approved"].ToString();
                approved = String.IsNullOrEmpty(approvedQuery) ? true : bool.Parse(approvedQuery);
            }
            
            bool hasMembershipQuery = Request.Query.Keys.Contains("hasMembership");
            bool hasMembership = true;

            if (hasMembershipQuery)
            {
                var membershipQuery = Request.Query["hasMembership"].ToString();
                hasMembership = String.IsNullOrEmpty(membershipQuery) ? true : bool.Parse(membershipQuery);
            }





            var userList = await _context.EndUser.Where(user =>
                (
                    (user.Email != null && user.Email.Contains(query))
                    || (
                        user.FirstName != null && 
                        (
                            query.Length > user.FirstName.Length ? 
                                query.Contains(user.FirstName.ToLower()) : 
                                user.FirstName.ToLower().Contains(query)
                        ))
                    || (user.LastName != null && (
                        query.Length > user.LastName.ToLower().Length ? 
                            query.Contains(user.LastName.ToLower())
                            : user.LastName.ToLower().Contains(query)
                        )
                    )
                )
            ).ToListAsync();
            Console.WriteLine("Query is {0}, Approved: {1}", query, approved);
            JsonArray users = new JsonArray();

            var guestMembership = await _context.Membership
                .Where(m => m.MembershipType == Enum.Parse<Membership.MembershipEnum>("Guest")).FirstOrDefaultAsync();
            
            
            
            foreach (var endUser in userList)
            {
                var membership = await _context.Membership.Where(membership => membership.Id == endUser.MembershipId).FirstOrDefaultAsync();
                if (membership != null)
                {
                    endUser.Membership = membership;
                }

                bool passUser = true;
                if (hasMembershipQuery)
                {
                    if (guestMembership != null && passUser)
                    {
                        passUser = hasMembership
                            ? endUser.MembershipId != guestMembership.Id
                            : endUser.MembershipId == guestMembership.Id;
                        
                        Console.WriteLine("Membership: {0}, Guest Membership: {1}, Has Membership: {2}", endUser.MembershipId, guestMembership.Id, hasMembership);
                    }
                }

                if (passedApprovedQuery)
                {
                    if (endUser.Approved != approved && passUser)
                    {
                        passUser = false;
                    }
                }

                if (passUser)
                {
                    users.Add(Methods.PublicFacingUser(endUser).Value);
                }
            }

            return Ok(users);
        }

        [Route("EndUser/Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn()
        {
            Console.WriteLine("Signing in...");
            StreamReader reader = new StreamReader(Request.Body);
            var bodyString = await reader.ReadToEndAsync();
            JObject body = JObject.Parse(bodyString);

            var email = body["email"]?.ToString();
            var password = body["password"]?.ToString();

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Email and password are required" } );
            }


            var existingUserList = await _context.EndUser.Where(e => e.Email == email).ToListAsync();
            if (existingUserList.Count == 0)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "This user does not exist in our system" } );
            }
            
            var existingUser = existingUserList[0];
            if (!BCrypt.Net.BCrypt.EnhancedVerify(password, existingUser.Password))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Incorrect password" } );
            }
            
            var byteArray = System.Text.Encoding.UTF8.GetBytes(existingUser.Id.ToString());
            var baseUser  = Convert.ToBase64String(byteArray);
                            
            Response.Cookies.Append("tc-session-user", baseUser, Methods.CreateCookieOptions());
            return  Ok(Json(new { user = Methods.PublicFacingUser(existingUser) }));
        }

        public IActionResult Register()
        {
            var membershipList = _context.Membership.Where(membership =>
                membership.MembershipType != Enum.Parse<Membership.MembershipEnum>("Guest")).ToList();
            return View(membershipList);
        }

        // GET: EndUser
        public async Task<IActionResult> Index()
        {
            var applicationDatabaseContext = _context.EndUser.Include(e => e.Membership);
            return View(await applicationDatabaseContext.ToListAsync());
        }

        // GET: EndUser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var endUser = await _context.EndUser
                .Include(e => e.Membership)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (endUser == null)
            {
                return NotFound();
            }

            return View(endUser);
        }

        [Route("EndUser/SendEmail/{email}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(string email)
        {
            int generatedCode = Methods.GenerateCode();
            string messageBody = "<h4 style='margin-bottom: 15px'>Hello from Together culture</h4>" +
                                   "<p>Please enter the code below to verify your email address. This code expires in 30 minutes.<p>" +
                                   $"<div style='margin: 15px 0; border-radius: 10px; background: rgba(259, 209, 209, .45); color: #481326;justify-content: center; padding: .5rem 1rem; text-align: center; width: fit-content; display: flex; font-weight: 700;'><span>{ generatedCode.ToString() }</span></div>";


            /*
             * Added the api key here for submission purposes, api keys are normally added
             * separately in uncommitted environment files that are not checked in source control
             */
            var apiKey = "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";
            //var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var fromEmail = new EmailAddress("osk103@student.aru.ac.uk", "TC");
            var subject = "Verify your email address";
            var toEmail = new EmailAddress(email, "Test user");
            
            
            var message = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, "Test text content",messageBody);
            var response = await client.SendEmailAsync(message);
            
            Console.WriteLine("Email: {0}", email);
            Console.WriteLine("Response: {0}", await response.Body.ReadAsStringAsync());
            Console.WriteLine("Message sent: {0}", generatedCode.ToString());

            var responseJson = Json(new
            {
                code = generatedCode.ToString(),
                createdAt = DateTime.Now,
            });
            return Ok(responseJson);
        }

        // GET: EndUser/Create
       /* public IActionResult Create()
        {
            ViewData["MembershipId"] = new SelectList(_context.Membership, "Id", "Id");
            return View();
        }*/

        // POST: EndUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("EndUser/Create/{accountType}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string accountType)
        {
            
            //System.Console.WriteLine("Creating end user: {0}", accountType);
            StreamReader reader = new StreamReader(Request.Body);
            var bodyString = await reader.ReadToEndAsync();
            JObject body = JObject.Parse(bodyString);
           // System.Console.WriteLine("String:" + bodyString);

            
            //return Ok(Json(new { user = "Test user "}));
            System.Console.WriteLine("Creating account type {0}", accountType);
            if (accountType == "Guest")
            {
                var endUser = new EndUser
                {
                    Email = body["email"]?.ToString(),
                    FirstName = body["firstName"]?.ToString(),
                    LastName = body["lastName"]?.ToString(),
                    Phone = body["phone"]?.ToString(),
                    Approved = true
                };

                var bodyDob = body["dateOfBirth"]?.ToString();

                if (bodyDob != null)
                {
                    var dateOfBirth = DateTime.Parse(bodyDob);
                    endUser.DateOfBirth = dateOfBirth;
                }

                var memberships = from membership in _context.Membership select membership;
                var existingMemberships = memberships.Where(membership => membership.MembershipType == Enum.Parse<Membership.MembershipEnum>("Guest"));
                var membershipList = await existingMemberships.ToListAsync();
                if (membershipList.Any())
                {
                    var membership = membershipList.First();
                    endUser.Membership = membership;
                    endUser.MembershipId = membership.Id;
                }

                
                var gender = body["gender"]?.ToString();
                if (gender != null)
                {
                    endUser.Gender = Enum.Parse<EndUser.GenderEnum>(gender);
                }
                
                System.Console.WriteLine("Valid model {0}", ModelState.IsValid);
                var users = from user in _context.EndUser select user;
                var existingUsers = users.Where(user => user.Email == endUser.Email);
                
                var userList = await existingUsers.ToListAsync();
                if (!userList.Any())
                {
                    endUser.CreatedAt = DateTime.Now;
                    endUser.UpdatedAt = DateTime.Now;
                    var createdUser = await _context.AddAsync(endUser);
                    await _context.SaveChangesAsync();
                    
                    return Ok(Json(new
                    {
                        user = Methods.PublicFacingUser(endUser)
                    }));
                }
                
                // System.Console.WriteLine("User already exists");
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "This user already exists in our system" });
            }
            else if (accountType == "Member")
            { 

                var password = body["password"]?.ToString();
                var endUser = new EndUser
                {
                    FirstName = body["firstName"]?.ToString(),
                    LastName = body["lastName"]?.ToString(),
                    Email = body["email"]?.ToString(),
                    Phone = body["phone"]?.ToString(),
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13),
                    SubscriptionDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                
                var bodyDob = body["dateOfBirth"]?.ToString();
                if (!String.IsNullOrEmpty(bodyDob))
                {
                    var dateOfBirth = DateTime.Parse(bodyDob);
                    endUser.DateOfBirth = dateOfBirth;
                }
                
                
                var existingUser = await _context.EndUser.FirstOrDefaultAsync(user => user.Email == endUser.Email);
                Membership? existingMembership = null;
                
                if (existingUser != null)
                {
                    Console.WriteLine("Membership Id: {0}", existingUser.MembershipId);
                    var membershipType = await _context.Membership.FirstOrDefaultAsync(m => m.Id == existingUser.MembershipId);


                    existingMembership = membershipType;
                    if (membershipType != null)
                    {
                                            
                        Console.WriteLine("Membership data: {0}", membershipType.Name);
                        if (membershipType.MembershipType != Membership.MembershipEnum.Guest)
                        {
                            Response.StatusCode = StatusCodes.Status400BadRequest;
                            return Json(new { message = "This user already exists in our system" });
                        }
                        
                    }

                }

                string? gender = body["gender"]?.ToString();
                if (gender != null)
                {
                    endUser.Gender = Enum.Parse<EndUser.GenderEnum>(gender);
                }
                
                
                string? requestMembership = body["membership"]?.ToString();
                Console.WriteLine("Request membership: {0}", requestMembership);
                if (!String.IsNullOrEmpty(requestMembership))
                {
                    int membershipId = int.Parse(requestMembership);
                    Console.WriteLine("Membership Id: {0}", membershipId);
                    Membership? membership = await _context.Membership.FirstOrDefaultAsync(m => m.Id == membershipId);
                    
                   Console.WriteLine("Fetching membership");

                    if (membership != null)
                    {
                        Console.WriteLine("Fetched membership: {0}", membership.Name);
                        endUser.Membership = membership;
                        endUser.MembershipId = membership.Id;
                    }
                }
                
                string? requestDiscount = body["discount"]?.ToString();
                if (!String.IsNullOrEmpty(requestDiscount))
                {
                    int discountId = int.Parse(requestDiscount);
                    Discount? discount = await _context.Discount.FirstOrDefaultAsync(d => d.Id == discountId);
                    
                    if (discount != null && existingUser != null)
                    {
                        var usedDiscount = await _context.DiscountUser.FirstOrDefaultAsync(d => d.DiscountId == discountId && d.EndUserId == existingUser.Id);
                        
                        if (usedDiscount == null)
                        {
                           await _context.DiscountUser.AddAsync(new DiscountUser
                            {
                                DiscountId = discount.Id,
                                Discount = discount,
                                EndUserId = existingUser.Id,
                                EndUser = existingUser,
                            });
                        }
                    }

                }



                if (existingMembership != null)
                {
                    if (existingMembership.MembershipType == Membership.MembershipEnum.Guest)
                    {
                        if (existingUser != null)
                        {
                            existingUser.Membership = endUser.Membership;
                            existingUser.MembershipId = endUser.MembershipId;
                            existingUser.SubscriptionDate = endUser.SubscriptionDate;
                            existingUser.UpdatedAt = DateTime.Now;
                            existingUser.Approved = false;
                            existingUser.Password = endUser.Password;
                            
                            await _context.SaveChangesAsync();
                            
                            var byteArray = System.Text.Encoding.UTF8.GetBytes(existingUser.Id.ToString());
                            var baseUser  = Convert.ToBase64String(byteArray);
                            
                            Response.Cookies.Append("tc-session-user", baseUser, Methods.CreateCookieOptions());
                            return  Ok(Json(new { user = Methods.PublicFacingUser(existingUser) }));
                            
                        }
                    }
                }
                
                
                var createdUser = await _context.EndUser.AddAsync(endUser);
                await _context.SaveChangesAsync();
                var endUserEntity = createdUser.Entity;
                var id = endUserEntity.Id.ToString();
                
                var textBytes = System.Text.Encoding.UTF8.GetBytes(id);
                var textString = Convert.ToBase64String(textBytes);
                
                Response.Cookies.Append("tc-session-user", textString, Methods.CreateCookieOptions());
                
                return Ok(Json(new { user = Methods.PublicFacingUser(endUserEntity) }));
                


            }

            //return RedirectToAction(nameof(Index));

            Response.StatusCode = StatusCodes.Status500InternalServerError;
             return Json(new { message = "Internal server error" });
             //ViewData["MembershipId"] = new SelectList(_context.Membership, "Id", "Id", endUser.MembershipId);
             // return View(endUser);
        }

        // GET: EndUser/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var endUser = await _context.EndUser.FindAsync(id);
            if (endUser == null)
            {
                return NotFound();
            }
            ViewData["MembershipId"] = new SelectList(_context.Membership, "Id", "Id", endUser.MembershipId);
            return View(endUser);
        }

        // POST: EndUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MembershipId,FirstName,LastName,Email,Password,Phone,Gender,DateOfBirth,SubscriptionDate,CheckIn,CheckOut,CreatedAt,UpdatedAt")] EndUser endUser)
        {
            if (id != endUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(endUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EndUserExists(endUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MembershipId"] = new SelectList(_context.Membership, "Id", "Id", endUser.MembershipId);
            return View(endUser);
        }

        // GET: EndUser/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var endUser = await _context.EndUser
                .Include(e => e.Membership)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (endUser == null)
            {
                return NotFound();
            }

            return View(endUser);
        }

        // POST: EndUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var endUser = await _context.EndUser.FindAsync(id);
            if (endUser != null)
            {
                _context.EndUser.Remove(endUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EndUserExists(int id)
        {
            return _context.EndUser.Any(e => e.Id == id);
        }
    }
}
