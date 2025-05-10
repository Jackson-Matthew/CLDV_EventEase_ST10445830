using Booking_Management_system.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.IO;
using Azure.Storage.Blobs;

namespace Booking_Management_system.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDBContext _context;

        public VenueController(ApplicationDBContext context)
        {
            _context = context;
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
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                        venue.IMAGE_URL = blobUrl;
                    }

                    _context.Venue.Add(venue);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Venue created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                return View(venue);
        }

        public IActionResult Edit(int id)
        {
            var venue = _context.Venue.Find(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
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
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);
                        venue.IMAGE_URL = blobUrl;
                    }
                    else
                    {

                    }
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Venue.Any(e => e.VENUE_ID == venue.VENUE_ID))
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
            return View(venue);
        }

        public IActionResult Delete(int id)
        {
            var venue = _context.Venue.Find(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venue.FindAsync(id);
            if (venue != null)
            {
                _context.Venue.Remove(venue);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            var venue = _context.Venue.Find(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        private async Task<string> UploadImageToBlobAsync(IFormFile file)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=storagesolutions;AccountKey=..."; // truncated for security
            var containerName = "storagesolutions";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(file.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }

    }
}

