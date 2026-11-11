
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TingkinWash.Models;
using TingkinWash.ViewModels;

namespace TingkinWash.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // Count registered customers
            var customers = await _userManager.GetUsersInRoleAsync("Customer");

            var model = new AdminDashboardViewModel
            {
                TotalCustomers = customers.Count,

                // Temporary values until Bookings table is created
                TotalBookings = 0,
                PendingBookings = 0,
                CompletedBookings = 0,
                TotalSales = 0
            };

            return View(model);
        }
    }
}
