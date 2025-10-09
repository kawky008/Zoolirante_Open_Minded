using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zoolirante_Open_Minded.Models;

namespace Zoolirante_Open_Minded.Controllers
{
    public class EventsController : Controller
    {
        private readonly ZooliranteDatabaseContext _context;

        public EventsController(ZooliranteDatabaseContext context)
        {
            _context = context;
        }

        // GET: Events
        public IActionResult Index()=> RedirectToAction(nameof(Ongoing));
        public async Task<IActionResult> Ongoing()
        {
			ViewData["BannerText"] = "View our events";
			var now = DateTime.Now;
            var list = await _context.Events
                .Where(e => e.StartTime <= now && e.EndTime >= now)
                .OrderBy(e => e.EndTime)
                .ToListAsync();
            ViewBag.Mode = "ongoing";
            return View("Index", list);
        }

        public async Task<IActionResult> Upcoming()
        {
			ViewData["BannerText"] = "Welcome to Zoolirante";
			var now = DateTime.Now;
            var list = await _context.Events
                .Where(e => e.StartTime > now)
                .OrderBy(e => e.StartTime)
                .ToListAsync();
            ViewBag.Mode = "upcoming";
            return View("Index", list);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,Title,Description,StartTime,EndTime,Capacity,Price,Location")] Event @event)
        {
            // Business validation (kept minimal and non-invasive)
            if (string.IsNullOrWhiteSpace(@event.Title))
                ModelState.AddModelError(nameof(@event.Title), "Title is required.");

            if (string.IsNullOrWhiteSpace(@event.Location))
                ModelState.AddModelError(nameof(@event.Location), "Location is required.");

            if (@event.StartTime >= @event.EndTime)
                ModelState.AddModelError(nameof(@event.EndTime), "End time must be after start time.");

            if (@event.Capacity.HasValue && @event.Capacity.Value < 0)
                ModelState.AddModelError(nameof(@event.Capacity), "Capacity cannot be negative.");

            if (@event.Price < 0)
                ModelState.AddModelError(nameof(@event.Price), "Price cannot be negative.");

            if (!ModelState.IsValid)
                return View(@event);

            _context.Add(@event);
            await _context.SaveChangesAsync();

            // Ensure the new event appears on the public Events page immediately:
            var now = DateTime.Now;
            if (@event.StartTime > now)
                return RedirectToAction(nameof(Upcoming));  // will show on Upcoming
            if (@event.StartTime <= now && @event.EndTime >= now)
                return RedirectToAction(nameof(Ongoing));   // will show on Ongoing

            // If the event was created already ended (edge case), default to Upcoming list
            return RedirectToAction(nameof(Upcoming));
        }


        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Title,Description,StartTime,EndTime,Capacity,Price,Location")] Event @event)
        {
            if (id != @event.EventId)
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
                    if (!EventExists(@event.EventId))
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

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
