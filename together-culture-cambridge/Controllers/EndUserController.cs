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
    public class EndUserController : Controller
    {
        private readonly ApplicationDatabaseContext _context;

        public EndUserController(ApplicationDatabaseContext context)
        {
            _context = context;
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

        // GET: EndUser/Create
        public IActionResult Create()
        {
            ViewData["MembershipId"] = new SelectList(_context.Membership, "Id", "Id");
            return View();
        }

        // POST: EndUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MembershipId,FirstName,LastName,Email,Password,Phone,Gender,DateOfBirth,SubscriptionDate,CheckIn,CheckOut,CreatedAt,UpdatedAt")] EndUser endUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(endUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MembershipId"] = new SelectList(_context.Membership, "Id", "Id", endUser.MembershipId);
            return View(endUser);
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
