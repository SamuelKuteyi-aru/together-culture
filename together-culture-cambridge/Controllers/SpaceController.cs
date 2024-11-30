using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using together_culture_cambridge.Data;
using together_culture_cambridge.Helpers;
using together_culture_cambridge.Models;

namespace together_culture_cambridge.Controllers
{
    public class SpaceController : Controller
    {
        private readonly ApplicationDatabaseContext _context;

        public SpaceController(ApplicationDatabaseContext context)
        {
            _context = context;
        }

        // GET: Space
        public async Task<IActionResult> Index()
        {
            int adminId = Methods.ReadAdminCookie(Request);
            int userId = Methods.ReadUserCookie(Request);
            
            bool validUser = adminId != -1 || userId != -1;
            if (!validUser)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }
            
            var spaces = await _context.Space.ToListAsync();
            return Ok(Methods.CreateSpaceList(spaces, _context));
        }

        [Route("Space/Delete/{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            int adminId = Methods.ReadAdminCookie(Request);
            if (adminId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }
            var space = await _context.Space.FindAsync(id);
            if (space == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new { message = "Space not found" });
            }
            
            _context.Space.Remove(space);
            await _context.SaveChangesAsync();
            var result = new JsonResult(new { message = "Space successfully deleted" });
            return Ok(result.Value);
        }

        [Route("Space/{id}")]
        [HttpPut]
        public async Task<IActionResult> Update(int id)
        {
            int adminId = Methods.ReadAdminCookie(Request);
            if (adminId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }
            
            
            var space = await _context.Space.FindAsync(id);
            if (space == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new { message = "Space not found" });
            }
            
            StreamReader reader = new StreamReader(Request.Body);
            var bodyString = await reader.ReadToEndAsync();
            JObject body = JObject.Parse(bodyString);
                
            space.RoomId = body["roomId"]?.ToString() ?? "";
            space.TotalSeats = int.Parse(body["totalSeats"]?.ToString() ?? "0");
            space.OpeningTime = DateTime.Parse(body["openingTime"]?.ToString() ?? "");
            space.ClosingTime = DateTime.Parse(body["closingTime"]?.ToString() ?? "");
            space.MinimumAccessLevel = Enum.Parse<Membership.MembershipEnum>(body["minimumAccessLevel"]?.ToString() ?? "");

            await _context.SaveChangesAsync();
            return Ok(Methods.CreateSpaceItem(space, _context).Value);
        }

        [Route("Space/Search")]
        [HttpGet]
        public async Task<IActionResult> Search()
        {
            int adminId = Methods.ReadAdminCookie(Request);
            int userId = Methods.ReadUserCookie(Request);
            
            bool validUser = adminId != -1 || userId != -1;
            if (!validUser)
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

            var spaceList = await _context.Space
                .Where(spaceItem => 
                    spaceItem.RoomId.ToLower().Contains(query))
                .ToListAsync();
            
            return Ok(Methods.CreateSpaceList(spaceList, _context));
        }

        // GET: Space/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var space = await _context.Space
                .FirstOrDefaultAsync(m => m.Id == id);
            if (space == null)
            {
                return NotFound();
            }

            return View(space);
        }

        // GET: Space/Create
       /* public IActionResult Create()
        {
            return View();
        }*/

        // POST: Space/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            int adminId = Methods.ReadAdminCookie(Request);
            if (adminId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }
            
            StreamReader reader = new StreamReader(Request.Body);
            var bodyString = await reader.ReadToEndAsync();
            JObject body = JObject.Parse(bodyString);
            
            var roomId = body["roomId"]?.ToString() ?? "";
            var totalSeats = int.Parse(body["totalSeats"]?.ToString() ?? "0");
            var openingTime = DateTime.Parse(body["openingTime"]?.ToString() ?? "");
            var closingTime = DateTime.Parse(body["closingTime"]?.ToString() ?? "");
            var minimumAccessLevel = Enum.Parse<Membership.MembershipEnum>(body["minimumAccessLevel"]?.ToString() ?? "");

            var space = new Space
            {
                RoomId = roomId,
                MinimumAccessLevel = minimumAccessLevel,
                TotalSeats = totalSeats,
                OpeningTime = openingTime,
                ClosingTime = closingTime,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            
           await _context.Space.AddAsync(space);
           await _context.SaveChangesAsync();

           return Ok(Methods.CreateSpaceItem(space, _context).Value);
        }

        // GET: Space/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var space = await _context.Space.FindAsync(id);
            if (space == null)
            {
                return NotFound();
            }
            return View(space);
        }

        // POST: Space/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MinimumAccessLevel,RoomId,AvailableSeats,OpeningTime,ClosingTime")] Space space)
        {
            if (id != space.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(space);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpaceExists(space.Id))
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
            return View(space);
        }

        // GET: Space/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var space = await _context.Space
                .FirstOrDefaultAsync(m => m.Id == id);
            if (space == null)
            {
                return NotFound();
            }

            return View(space);
        }

        // POST: Space/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var space = await _context.Space.FindAsync(id);
            if (space != null)
            {
                _context.Space.Remove(space);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpaceExists(int id)
        {
            return _context.Space.Any(e => e.Id == id);
        }
    }
}
