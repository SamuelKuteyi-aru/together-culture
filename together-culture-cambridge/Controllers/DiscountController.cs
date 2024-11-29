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

namespace together_culture_cambridge.Controllers
{
    public class DiscountController : Controller
    {
        private readonly ApplicationDatabaseContext _context;

        public DiscountController(ApplicationDatabaseContext context)
        {
            _context = context;
        }
        
        //POST: Discount/Verify
        [Route("/Discount/Verify")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify()
        {
            StreamReader reader = new StreamReader(Request.Body);
            var bodyString = await reader.ReadToEndAsync();
            JObject body = JObject.Parse(bodyString);
            
            var code = body["code"]?.ToString();
            if (String.IsNullOrEmpty(code))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { success = false, message = "The code is missing from this request." });
            }
            
            var discounts = _context.Discount.ToList();
            var existingDiscount = discounts.FirstOrDefault(d => d.Code == code);

            if (existingDiscount == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new { success = false, message = "The discount code does not exist." });
            }
            
            var expirationDate = existingDiscount.ExpirationDate;
            if (expirationDate < DateTime.Now)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { success = false, message = "The discount code is expired." });
            }

            return Ok(Json(new { discount = existingDiscount }));
        }

        // GET: Discount
        public async Task<IActionResult> Index()
        {
            return View(await _context.Discount.ToListAsync());
        }

        // GET: Discount/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        // GET: Discount/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discount/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Percentage,CreatedAt,ExpirationDate")] Discount discount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(discount);
        }

        // GET: Discount/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discount.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }

        // POST: Discount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Percentage,CreatedAt,ExpirationDate")] Discount discount)
        {
            if (id != discount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountExists(discount.Id))
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
            return View(discount);
        }

        // GET: Discount/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discount
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        // POST: Discount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discount = await _context.Discount.FindAsync(id);
            if (discount != null)
            {
                _context.Discount.Remove(discount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscountExists(int id)
        {
            return _context.Discount.Any(e => e.Id == id);
        }
    }
}
