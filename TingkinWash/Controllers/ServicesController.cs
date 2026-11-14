using Microsoft.AspNetCore.Mvc;

namespace TingkinWash.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
