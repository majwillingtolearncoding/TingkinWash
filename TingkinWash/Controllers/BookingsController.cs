
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TingkinWash.Data;
using TingkinWash.Models;

namespace TingkinWash.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        private static readonly string[] AllowedServices =
        {
            "Wash & Fold",
            "Dry Cleaning",
            "Wash, Dry & Fold",
            "Express Laundry"
        };

        public BookingsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // MY BOOKINGS
        public async Task<IActionResult> Index(
            string? search, string status = "All")
        {
            var userId = _userManager.GetUserId(User);

            var query = _context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                if (int.TryParse(search.TrimStart('#'), out int bookingId))
                {
                    query = query.Where(b =>
                        b.BookingId == bookingId ||
                        b.ServiceName.Contains(search));
                }
                else
                {
                    query = query.Where(b =>
                        b.ServiceName.Contains(search));
                }
            }

            if (!string.IsNullOrWhiteSpace(status) &&
                status != "All")
            {
                query = query.Where(b => b.Status == status);
            }

            var bookings = await query
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            ViewBag.Search = search ?? "";
            ViewBag.Status = status;

            return View(bookings);
        }

        // CREATE BOOKING - GET
        [HttpGet]
        public IActionResult Create(string? service)
        {
            if (string.IsNullOrWhiteSpace(service) ||
                !AllowedServices.Contains(service))
            {
                return RedirectToAction("Index", "Services");
            }

            var booking = new Booking
            {
                ServiceName = service,
                PickupDate = DateTime.Today.AddDays(1),
                PickupTime = new TimeSpan(9, 0, 0)
            };

            return View(booking);
        }

        // CREATE BOOKING - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (!AllowedServices.Contains(booking.ServiceName))
            {
                ModelState.AddModelError(
                    nameof(Booking.ServiceName),
                    "Invalid laundry service.");
            }

            if (booking.PickupDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(Booking.PickupDate),
                    "Choose today or a future date.");
            }

            // These values are assigned by the server/admin.
            ModelState.Remove(nameof(Booking.UserId));
            ModelState.Remove(nameof(Booking.Weight));
            ModelState.Remove(nameof(Booking.TotalPrice));
            ModelState.Remove(nameof(Booking.Status));
            ModelState.Remove(nameof(Booking.BookingDate));

            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            booking.UserId = _userManager.GetUserId(User)!;
            booking.Weight = null;
            booking.TotalPrice = null;
            booking.Status = "Pending";
            booking.BookingDate = DateTime.Now;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Your booking was submitted successfully!";

            return RedirectToAction(nameof(Index));
        }

        // VIEW BOOKING DETAILS
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var booking = await _context.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(b =>
                    b.BookingId == id &&
                    b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // EDIT BOOKING - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.BookingId == id &&
                    b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status != "Pending")
            {
                TempData["Error"] =
                    "Only pending bookings can be edited.";

                return RedirectToAction(nameof(Index));
            }

            return View(booking);
        }

        // EDIT BOOKING - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("BookingId,PickupDate,PickupTime,PickupAddress")]
            Booking updatedBooking)
        {
            var userId = _userManager.GetUserId(User);

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.BookingId == id &&
                    b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status != "Pending")
            {
                TempData["Error"] =
                    "Only pending bookings can be edited.";

                return RedirectToAction(nameof(Index));
            }

            if (updatedBooking.PickupDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(Booking.PickupDate),
                    "Choose today or a future date.");
            }

            if (string.IsNullOrWhiteSpace(updatedBooking.PickupAddress))
            {
                ModelState.AddModelError(
                    nameof(Booking.PickupAddress),
                    "Pickup address is required.");
            }

            // Validate only fields the customer can edit.
            ModelState.Remove(nameof(Booking.UserId));
            ModelState.Remove(nameof(Booking.ServiceName));
            ModelState.Remove(nameof(Booking.Weight));
            ModelState.Remove(nameof(Booking.TotalPrice));
            ModelState.Remove(nameof(Booking.Status));
            ModelState.Remove(nameof(Booking.BookingDate));

            if (!ModelState.IsValid)
            {
                updatedBooking.ServiceName = booking.ServiceName;
                return View(updatedBooking);
            }

            booking.PickupDate = updatedBooking.PickupDate;
            booking.PickupTime = updatedBooking.PickupTime;
            booking.PickupAddress = updatedBooking.PickupAddress;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Booking updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        // CANCEL BOOKING
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.BookingId == id &&
                    b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status != "Pending")
            {
                TempData["Error"] =
                    "Only pending bookings can be cancelled.";

                return RedirectToAction(nameof(Index));
            }

            booking.Status = "Cancelled";

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Booking cancelled successfully!";

            return RedirectToAction(nameof(Index));
        }

        // DELETE BOOKING
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.BookingId == id &&
                    b.UserId == userId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Status != "Cancelled")
            {
                TempData["Error"] =
                    "Only cancelled bookings can be deleted.";

                return RedirectToAction(nameof(Index));
            }

            _context.Bookings.Remove(booking);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Booking deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
