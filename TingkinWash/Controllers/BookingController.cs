using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TingkinWash.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}