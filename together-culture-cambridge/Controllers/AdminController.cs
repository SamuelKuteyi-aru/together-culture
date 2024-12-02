using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using together_culture_cambridge.Data;
using together_culture_cambridge.Models;
using Newtonsoft.Json.Linq;
using together_culture_cambridge.Helpers;

namespace together_culture_cambridge.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDatabaseContext _context;

        public AdminController(ApplicationDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("Admin/Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn()
        {
            StreamReader reader = new StreamReader(Request.Body);
            var bodyStr = await reader.ReadToEndAsync();
            JObject body = JObject.Parse(bodyStr);

            var email = body["email"]?.ToString();
            var password = body["password"]?.ToString();

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Email and password are required" });
            }


            var existingAdminList = await _context.Admin.Where(e => e.Email == email).ToListAsync();
            if (existingAdminList.Count == 0)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "This user does not exist in our system" });
            }

            var admin = existingAdminList[0];
            if (!BCrypt.Net.BCrypt.EnhancedVerify(password, admin.Password))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Incorrect password" });
            }

            var byteArray = System.Text.Encoding.UTF8.GetBytes(admin.Id.ToString());
            var baseAdmin = Convert.ToBase64String(byteArray);

            Response.Cookies.Append("tc-session-admin", baseAdmin, Methods.CreateCookieOptions());
            return Ok(new JsonResult(new
            {
                id = admin.Id,
                name = admin.FirstName + " " + admin.LastName
            }));
        }

        // GET: Dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Events()
        {
            return View();
        }

        public IActionResult Spaces()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LogOut()
        {
            if (Request.Cookies["tc-session-admin"] != null)
            {
                Response.Cookies.Delete("tc-session-admin");
            }
            
            return RedirectToAction("Login", "Admin");
        }

        [HttpPost]
        [Route("Admin/Approve/{userId}")]
        public async Task<IActionResult> Approve(int userId)
        {
            int adminId = Methods.ReadAdminCookie(Request);
            if (adminId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }
            
            var existingUser = await _context.EndUser.Where(user => user.Id == userId).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new { message = "User not found" });
            }
            
            existingUser.Approved = true;
            await _context.SaveChangesAsync();
            
            
            Console.WriteLine("------User approved--------");

            return Ok(Methods.PublicFacingUser(existingUser));
        }

        // GET: Admin
        [HttpGet]
        [Route("/Admin")]
        public async Task<IActionResult> Get()
        {
            
            int adminId = Methods.ReadAdminCookie(Request);
            if (adminId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }
            
            var admin = await _context.Admin.FindAsync(adminId);

            if (admin == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request. Admin not found" });
            }

            return Ok(Methods.PublicFacingAdmin(admin));
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Admin.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,Password,CreatedAt")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Password,CreatedAt")] Admin admin)
        {
            if (id != admin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id))
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
            return View(admin);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admin.FindAsync(id);
            if (admin != null)
            {
                _context.Admin.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admin.Any(e => e.Id == id);
        }
    }
}
