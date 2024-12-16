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
    public class SpaceBookingController : Controller
    {
        private readonly ApplicationDatabaseContext _context;

        public SpaceBookingController(ApplicationDatabaseContext context)
        {
            _context = context;
        }

        // GET: SpaceBooking
        public async Task<IActionResult> Index()
        {
            var applicationDatabaseContext = _context.SpaceBooking.Include(s => s.EndUser).Include(s => s.Space);
            return View(await applicationDatabaseContext.ToListAsync());
        }

        // GET: SpaceBooking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spaceBooking = await _context.SpaceBooking
                .Include(s => s.EndUser)
                .Include(s => s.Space)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spaceBooking == null)
            {
                return NotFound();
            }

            return View(spaceBooking);
        }

        public async Task<IActionResult> Cancel()
        {
            var userId = Methods.ReadUserCookie(Request);
            if (userId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }
            
            StreamReader reader = new StreamReader(Request.Body);
            var bodyString = await reader.ReadToEndAsync();
            JObject body = JObject.Parse(bodyString);
            
            var bodySpaceId = body["spaceId"];
            if (bodySpaceId == null) {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Space ID is required" });
            }
            
            var spaceId = int.Parse(bodySpaceId.ToString());
            var space = await _context.Space.FindAsync(spaceId);
            if (space == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new { message = "Space not found" });
            }
            
            var spaceBooking = await _context.SpaceBooking.Where(booking => booking.SpaceId == space.Id && booking.EndUserId == userId).FirstOrDefaultAsync();
            if (spaceBooking == null)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Space booking not found" });
            }
            
            _context.SpaceBooking.Remove(spaceBooking);
            await _context.SaveChangesAsync();

            return Json(new { message = "Space booking has been cancelled" });
        }


        // POST: SpaceBooking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create()
        {

            var userId = Methods.ReadUserCookie(Request);
            if (userId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }
            
            StreamReader reader = new StreamReader(Request.Body);
            var bodyString = await reader.ReadToEndAsync();
            JObject body = JObject.Parse(bodyString);

            var bodySpaceId = body["spaceId"];
            var bodyBookingDate = body["bookingDate"];
            if (bodySpaceId == null || bodyBookingDate == null) {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Space ID and Booking Date are required" });
            }
            
            var spaceId = int.Parse(bodySpaceId.ToString());
            var space = await _context.Space.FindAsync(spaceId);
            if (space == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new { message = "Space not found" });
            }

            var spaceBookings = Methods.GetSpaceBookings(space, _context);
            if (spaceBookings.Count >= space.TotalSeats)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Space is full" });
            }

            var openingHours = space.OpeningTime.Hour;
            var closingHours = space.ClosingTime.Hour;

            var bookingDateTime = DateTime.Parse(bodyBookingDate.ToString());
            var dateHours = bookingDateTime.Hour;
            var dateCompare = DateTime.Compare(bookingDateTime, DateTime.Now);
            if (dateCompare < 0)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Invalid booking date" });
            }
            

            if (!(dateHours >= openingHours && dateHours <= closingHours))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "The space is not open at selected time" });
            }

            var spaceBooking = new SpaceBooking
            {
                EndUserId = userId,
                SpaceId = space.Id,
                BookingDate = bookingDateTime
            };
            
            _context.SpaceBooking.Add(spaceBooking);
            await _context.SaveChangesAsync();

            var spaceItem = Methods.CreateSpaceItem(space, _context, spaceBooking);
            return Ok(spaceItem.Value);
        }

       

        // GET: SpaceBooking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spaceBooking = await _context.SpaceBooking
                .Include(s => s.EndUser)
                .Include(s => s.Space)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (spaceBooking == null)
            {
                return NotFound();
            }

            return View(spaceBooking);
        }

        // POST: SpaceBooking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var spaceBooking = await _context.SpaceBooking.FindAsync(id);
            if (spaceBooking != null)
            {
                _context.SpaceBooking.Remove(spaceBooking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpaceBookingExists(int id)
        {
            return _context.SpaceBooking.Any(e => e.Id == id);
        }
    }
}
