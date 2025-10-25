using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    public class RoomController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();
        [HttpGet]
        public IActionResult Stream() => View();
    }
}
