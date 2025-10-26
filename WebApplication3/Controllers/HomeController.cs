using Microsoft.AspNetCore.Mvc;

namespace WebRtcApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}