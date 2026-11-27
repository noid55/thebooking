using Microsoft.AspNetCore.Mvc;

namespace thebooking.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Shown when a logged-in user tries to do something they do not have permission to do
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}