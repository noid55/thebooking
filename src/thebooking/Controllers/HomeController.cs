using Microsoft.AspNetCore.Mvc;

namespace thebooking.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("The home page was accessed.");
            return View();
        }

        // Shown when a logged-in user tries to do something they do not have permission to do
        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Access denied for {Path}.", HttpContext.Request.Query["ReturnUrl"].ToString());
            return View();
        }
    }
}