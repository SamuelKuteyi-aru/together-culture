using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using together_culture_cambridge.Data;
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

        // GET: EventBooking/Create
        public IActionResult Create()
        {
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id");
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Id");
            return View();
        }

        // POST: EventBooking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EventId,EndUserId")] EventBooking eventBooking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventBooking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id", eventBooking.EndUserId);
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Id", eventBooking.EventId);
            return View(eventBooking);
        }

        // GET: EventBooking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventBooking = await _context.EventBooking.FindAsync(id);
            if (eventBooking == null)
            {
                return NotFound();
            }
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id", eventBooking.EndUserId);
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Id", eventBooking.EventId);
            return View(eventBooking);
        }

        // POST: EventBooking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventId,EndUserId")] EventBooking eventBooking)
        {
            if (id != eventBooking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventBooking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventBookingExists(eventBooking.Id))
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
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id", eventBooking.EndUserId);
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Id", eventBooking.EventId);
            return View(eventBooking);
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
