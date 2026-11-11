using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TingkinWash.Models;
using TingkinWash.ViewModels;

namespace TingkinWash.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var model = new UserDashboardViewModel
            {
                FullName = user.FullName,
                Username = user.UserName ?? "",
                PhoneNumber = user.PhoneNumber ?? "",

                TotalBookings = 0,
                PendingBookings = 0,
                CompletedBookings = 0
            };

            return View(model);
        }
    }
}