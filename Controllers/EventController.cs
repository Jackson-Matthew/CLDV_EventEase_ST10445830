using Booking_Management_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
namespace Booking_Management_system.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDBContext _context;
        public EventController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Event.Include(e => e.Venue).ToListAsync();
            return View(events);
        }
        public IActionResult Create()
        {
            ViewBag.VenueList = new SelectList(_context.Venue, "VENUE_ID", "VENUE_NAME");
            ViewData["VenueList"] = new SelectList(_context.Venue, "VENUE_ID", "VENUE_NAME");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EVENT_ID,EVENT_NAME,EVENT_DATE,DESCRIPTION,VENUE_ID")] Event @event)
        {

            _context.Event.Add(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        public IActionResult Details(int id)
        {
            var @event = _context.Event.Include(e => e.Venue).FirstOrDefault(e => e.EVENT_ID == id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        [HttpPost]
        public async Task<IActionResult> DetailsConfirmed(int id)
        {
            var @event = await _context.Event.Include(e => e.Venue).FirstOrDefaultAsync(e => e.EVENT_ID == id);
            if (@event != null)
            {
                return View("Details", @event);
            }
            return NotFound();
        }

        public IActionResult Delete(int id)
        {
            var @event = _context.Event.Include(e => e.Venue).FirstOrDefault(e => e.EVENT_ID == id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            if (@event != null)
            {
                _context.Event.Remove(@event);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewBag.VenueList = new SelectList(_context.Venue, "VENUE_ID", "VENUE_NAME");
            ViewData["VenueList"] = new SelectList(_context.Venue, "VENUE_ID", "VENUE_NAME");
            return View(@event);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EVENT_ID,EVENT_NAME,EVENT_DATE,DESCRIPTION,VENUE_ID")] Event @event)
        {
            if (id != @event.EVENT_ID)
            {
                return NotFound();
            }
            _context.Update(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            return View(@event);

        }
    }
}