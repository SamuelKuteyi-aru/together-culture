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
    public class DiscountUserController : Controller
    {
        private readonly ApplicationDatabaseContext _context;

        public DiscountUserController(ApplicationDatabaseContext context)
        {
            _context = context;
        }

        // GET: DiscountUser
        public async Task<IActionResult> Index()
        {
            var applicationDatabaseContext = _context.DiscountUser.Include(d => d.Discount).Include(d => d.EndUser);
            return View(await applicationDatabaseContext.ToListAsync());
        }

        // GET: DiscountUser/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountUser = await _context.DiscountUser
                .Include(d => d.Discount)
                .Include(d => d.EndUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discountUser == null)
            {
                return NotFound();
            }

            return View(discountUser);
        }

        // GET: DiscountUser/Create
        public IActionResult Create()
        {
            ViewData["DiscountId"] = new SelectList(_context.Discount, "Id", "Id");
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id");
            return View();
        }

        // POST: DiscountUser/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DiscountId,EndUserId")] DiscountUser discountUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discountUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiscountId"] = new SelectList(_context.Discount, "Id", "Id", discountUser.DiscountId);
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id", discountUser.EndUserId);
            return View(discountUser);
        }

        // GET: DiscountUser/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountUser = await _context.DiscountUser.FindAsync(id);
            if (discountUser == null)
            {
                return NotFound();
            }
            ViewData["DiscountId"] = new SelectList(_context.Discount, "Id", "Id", discountUser.DiscountId);
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id", discountUser.EndUserId);
            return View(discountUser);
        }

        // POST: DiscountUser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DiscountId,EndUserId")] DiscountUser discountUser)
        {
            if (id != discountUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discountUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountUserExists(discountUser.Id))
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
            ViewData["DiscountId"] = new SelectList(_context.Discount, "Id", "Id", discountUser.DiscountId);
            ViewData["EndUserId"] = new SelectList(_context.EndUser, "Id", "Id", discountUser.EndUserId);
            return View(discountUser);
        }

        // GET: DiscountUser/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discountUser = await _context.DiscountUser
                .Include(d => d.Discount)
                .Include(d => d.EndUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discountUser == null)
            {
                return NotFound();
            }

            return View(discountUser);
        }

        // POST: DiscountUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discountUser = await _context.DiscountUser.FindAsync(id);
            if (discountUser != null)
            {
                _context.DiscountUser.Remove(discountUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountUserExists(int id)
        {
            return _context.DiscountUser.Any(e => e.Id == id);
        }
    }
}
