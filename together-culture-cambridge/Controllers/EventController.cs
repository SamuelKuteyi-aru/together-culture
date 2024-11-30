using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
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
    public class EventController : Controller
    {
        private readonly ApplicationDatabaseContext _context;

        public EventController(ApplicationDatabaseContext context)
        {
            _context = context;
        }

        // GET: Event
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

            var events = await  _context.Event.ToListAsync();
           

            return Ok(Methods.CreateEventList(events, _context));
        }



        [Route("Event/Search")]
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
            
          
            var eventList = await  _context.Event.Where(eventItem => 
                eventItem.Name.ToLower().Contains(query) || 
                eventItem.Description.ToLower().Contains(query) ||
                eventItem.Address.ToLower().Contains(query)
            ).ToListAsync();
            
            return Ok(Methods.CreateEventList(eventList, _context));
        }
        
        
      
        

        
        [Route("Event/{id}")]
        [HttpPut]

        public async Task<IActionResult> Update(int id)
        {
            int adminId = Methods.ReadAdminCookie(Request);
            if (adminId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new { message = "Event not found" });
            }
            
            
            StreamReader reader = new StreamReader(Request.Body);
            var bodyString = await reader.ReadToEndAsync();
            JObject body = JObject.Parse(bodyString);
            
            
            @event.Name = body["name"]?.ToString() ?? @event.Name;
            @event.Description = body["description"]?.ToString() ?? @event.Description;
            @event.Address = body["address"]?.ToString() ?? @event.Address;
            @event.TotalSpaces = int.Parse(body["totalSpaces"]?.ToString() ?? @event.TotalSpaces.ToString());

            var ticketPrice = body["ticketPrice"]?.ToString();
            if (ticketPrice != null)
            {
                @event.TicketPrice = double.Parse(ticketPrice);
            }

            
            var startTime = body["startTime"]?.ToString();
            if (startTime != null)
            {
                @event.StartTime = DateTime.Parse(startTime);
            }

            var endTime = body["endTime"]?.ToString();
            if (endTime != null)
            {
                @event.EndTime = DateTime.Parse(endTime);
            }
            
            await _context.SaveChangesAsync();

            return Ok(Methods.CreateEventItem(@event, _context).Value);
        }

        // GET: Event/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        
        [HttpPost]
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

            var eventData = new Event
            {
               Name = body["name"]?.ToString() ?? "",
               Address = body["address"]?.ToString() ?? "",
               Description = body["description"]?.ToString() ?? "",
               TotalSpaces = int.Parse(body["totalSpaces"]?.ToString() ?? "0"),
               TicketPrice = double.Parse(body["ticketPrice"]?.ToString() ?? "0"),
               StartTime = DateTime.Parse(body["startTime"]?.ToString() ?? ""),
               EndTime = DateTime.Parse(body["endTime"]?.ToString() ?? ""),
               CreatedAt = DateTime.Now,
               UpdatedAt = DateTime.Now
            };
            
            var addedEvent = await _context.Event.AddAsync(eventData);
            /*if (ModelState.IsValid)
            {
                _context.Add();
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }*/
            
            await _context.SaveChangesAsync();
            
            Console.WriteLine("Event created");
            return Ok(addedEvent.Entity);
        }

        // GET: Event/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Event/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,TotalSpaces,TicketPrice,StartTime,EndTime,CreatedAt,UpdatedAt")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            return View(@event);
        }

        // GET: Event/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Event/Delete/5
        [HttpDelete, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            int adminId = Methods.ReadAdminCookie(Request);
            if (adminId == -1)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { message = "Unauthorized request" });
            }
            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
              Response.StatusCode = StatusCodes.Status404NotFound;
              return Json(new { message = "Event not found" });
            }

            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            var result = new JsonResult(new { message = "Event successfully deleted" });
            return Ok(result.Value);
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }
    }
}
