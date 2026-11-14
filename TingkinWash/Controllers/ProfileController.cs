using Microsoft.AspNetCore.Mvc;

namespace TingkinWash.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
