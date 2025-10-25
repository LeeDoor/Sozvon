using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;

namespace Server.Controllers  // ← ИЗМЕНИЛОСЬ!
{
    public class VideoCallController : Controller
    {
        private readonly IHubContext<WebRTCHub> _hubContext;

        public VideoCallController(IHubContext<WebRTCHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateRoom()
        {
            var roomId = Guid.NewGuid().ToString("N")[..8];
            return Json(new { roomId });
        }
    }
}