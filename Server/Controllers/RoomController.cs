using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    public class RoomController : Controller
    {
        [HttpGet]
        [Authorize]
        public IActionResult Index() => View(User.Identity.Name);
    }
}
