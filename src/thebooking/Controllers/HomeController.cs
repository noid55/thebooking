using Microsoft.AspNetCore.Mvc;

namespace thebooking.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}