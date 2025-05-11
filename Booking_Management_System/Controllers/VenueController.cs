using Booking_Management_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Booking_Management_system.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private const string ContainerName = "storagesolutions";

        public VenueController(ApplicationDBContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<IActionResult> Index()
        {
            var venues = await _context.Venue.ToListAsync();
            return View(venues);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                if (venue.ImageFile != null)
                {
                    venue.IMAGE_URL = await UploadImageToBlobAsync(venue.ImageFile);
                }

                _context.Venue.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        public IActionResult Edit(int id)
        {
            var venue = _context.Venue.Find(id);
            return venue == null ? NotFound() : View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VENUE_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        venue.IMAGE_URL = await UploadImageToBlobAsync(venue.ImageFile);
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue Edited successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Venue.Any(e => e.VENUE_ID == venue.VENUE_ID))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(venue);
        }

        public IActionResult Delete(int id)
        {
            var venue = _context.Venue.Find(id);
            return venue == null ? NotFound() : View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            if (venue == null) return NotFound();

            var hasBookings = await _context.Booking.AnyAsync(b => b.VENUE_ID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue as it has existing bookings";            
                return RedirectToAction(nameof(Index));
            }
            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var venue = _context.Venue.Find(id);
            return venue == null ? NotFound() : View(venue);
        }

        private async Task<string> UploadImageToBlobAsync(IFormFile file)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var blobClient = containerClient.GetBlobClient(blobName);

                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(
                        stream,
                        new BlobUploadOptions
                        {
                            HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType }
                        });
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
               
                throw new ApplicationException("Image upload failed", ex);
            }
        }
    }
}