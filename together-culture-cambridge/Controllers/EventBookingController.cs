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
    public class EventBookingController : Controller
    {
        private readonly ApplicationDatabaseContext _context;

        public EventBookingController(ApplicationDatabaseContext context)
        {
            _context = context;
        }

        // GET: EventBooking
        public async Task<IActionResult> Index()
        {
            var applicationDatabaseContext = _context.EventBooking.Include(e => e.EndUser).Include(e => e.Event);
            return View(await applicationDatabaseContext.ToListAsync());
        }

        // GET: EventBooking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventBooking = await _context.EventBooking
                .Include(e => e.EndUser)
                .Include(e => e.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventBooking == null)
            {
                return NotFound();
            }

            return View(eventBooking);
        }


        [HttpPost]
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

            var bodyEventId = body["eventId"];
            if (bodyEventId == null) {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Event ID is required" });
            }

            var eventId = int.Parse(bodyEventId.ToString());
            var @event = await  _context.Event.FindAsync(eventId);
            if (@event == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new { message = "Event not found" });
            }
            
            var eventBooking = await _context.EventBooking.Where(booking => booking.EndUserId == userId && booking.EventId == eventId).FirstOrDefaultAsync();
            if (eventBooking == null)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Event booking not found" });
            }

            _context.EventBooking.Remove(eventBooking);
            await _context.SaveChangesAsync();
            
            return Json(new { message = "Event booking has been cancelled" });
        }

        // POST: EventBooking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
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

            var bodyEventId = body["eventId"];
            if (bodyEventId == null) {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Event ID is required" });
            }

            var eventId = int.Parse(bodyEventId.ToString());
            var @event = await  _context.Event.FindAsync(eventId);
            if (@event == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new { message = "Event not found" });
            }
            
            var bookingList = await _context.EventBooking.Where(x => x.EventId == @event.Id).ToListAsync();
            if (bookingList.Count >= @event.TotalSpaces)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { message = "Event is full" });
            }

            var eventBooking = new EventBooking
            {
                EventId = eventId,
                EndUserId = userId
            };
            
            _context.Add(eventBooking);
            await _context.SaveChangesAsync();


            var eventItem = Methods.CreateEventItem(@event, _context);
            return Ok(eventItem.Value);
        }

     

        // GET: EventBooking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventBooking = await _context.EventBooking
                .Include(e => e.EndUser)
                .Include(e => e.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventBooking == null)
            {
                return NotFound();
            }

            return View(eventBooking);
        }

        // POST: EventBooking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventBooking = await _context.EventBooking.FindAsync(id);
            if (eventBooking != null)
            {
                _context.EventBooking.Remove(eventBooking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventBookingExists(int id)
        {
            return _context.EventBooking.Any(e => e.Id == id);
        }
    }
}
