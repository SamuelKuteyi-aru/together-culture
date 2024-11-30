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

        // GET: SpaceBooking/Create
        public IActionResult Create()
        {
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id");
            ViewData["SpaceId"] = new SelectList(_context.Space, "Id", "Id");
            return View();
        }

        // POST: SpaceBooking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EndUserId,SpaceId,BookingDate")] SpaceBooking spaceBooking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(spaceBooking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id", spaceBooking.EndUserId);
            ViewData["SpaceId"] = new SelectList(_context.Space, "Id", "Id", spaceBooking.SpaceId);
            return View(spaceBooking);
        }

        // GET: SpaceBooking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spaceBooking = await _context.SpaceBooking.FindAsync(id);
            if (spaceBooking == null)
            {
                return NotFound();
            }
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id", spaceBooking.EndUserId);
            ViewData["SpaceId"] = new SelectList(_context.Space, "Id", "Id", spaceBooking.SpaceId);
            return View(spaceBooking);
        }

        // POST: SpaceBooking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EndUserId,SpaceId,BookingDate")] SpaceBooking spaceBooking)
        {
            if (id != spaceBooking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(spaceBooking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpaceBookingExists(spaceBooking.Id))
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
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id", spaceBooking.EndUserId);
            ViewData["SpaceId"] = new SelectList(_context.Space, "Id", "Id", spaceBooking.SpaceId);
            return View(spaceBooking);
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
