using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Booking_Management_system.Models;

namespace Booking_Management_system.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDBContext _context;

        public BookingsController(ApplicationDBContext context)
        {
            _context = context;
        }

      
        public async Task<IActionResult> Index(string searchString)
        {
            var bookings = _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b =>
                b.Venue.VENUE_NAME.Contains(searchString) ||
                b.Event.EVENT_NAME.Contains(searchString));
            }
            return View(await bookings.ToArrayAsync());

        }

       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BOOKING_ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

     
        public IActionResult Create()
        {
            ViewData["EVENT_ID"] = new SelectList(_context.Event, "EVENT_ID", "EVENT_NAME");
            ViewData["VENUE_ID"] = new SelectList(_context.Venue, "VENUE_ID", "LOCATION");
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BOOKING_ID,BOOKING_DATE,EVENT_ID,VENUE_ID")] Booking booking)
        {
            var selectedEvent = await _context.Event.FirstOrDefaultAsync(e => e.EVENT_ID == booking.EVENT_ID);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event not found.");
                ViewData["Events"] = _context.Event.ToList();
                ViewData["Venues"] = _context.Venue.ToList();
                return View(booking);
            }

          
            var conflict = await _context.Booking
                .Include(b => b.Event)
                .AnyAsync(b => b.VENUE_ID == booking.VENUE_ID &&
                               b.Event.EVENT_DATE.Date == selectedEvent.EVENT_DATE.Date);

            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for that date.");
                ViewData["Events"] = _context.Event.ToList();
                ViewData["Venues"] = _context.Venue.ToList();
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EVENT_ID"] = new SelectList(_context.Event, "EVENT_ID", "EVENT_NAME", booking.EVENT_ID);
            ViewData["VENUE_ID"] = new SelectList(_context.Venue, "VENUE_ID", "LOCATION", booking.VENUE_ID);
            return View(booking);
        }

    
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EVENT_ID"] = new SelectList(_context.Event, "EVENT_ID", "EVENT_NAME", booking.EVENT_ID);
            ViewData["VENUE_ID"] = new SelectList(_context.Venue, "VENUE_ID", "LOCATION", booking.VENUE_ID);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BOOKING_ID,BOOKING_DATE,EVENT_ID,VENUE_ID")] Booking booking)
        {
            if (id != booking.BOOKING_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BOOKING_ID))
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
            ViewData["EVENT_ID"] = new SelectList(_context.Event, "EVENT_ID", "EVENT_NAME", booking.EVENT_ID);
            ViewData["VENUE_ID"] = new SelectList(_context.Venue, "VENUE_ID", "LOCATION", booking.VENUE_ID);
            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BOOKING_ID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BOOKING_ID == id);
        }
    }
}
