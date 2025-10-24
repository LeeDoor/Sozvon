
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.SignalR;

namespace SignalRApp
{
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("Receive", message);
        }
    }
}


namespace Server.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Register() => View();
        [HttpPost]
        public IActionResult RegisterPost()
        {
            Console.WriteLine("RegisterPost has been activated");
            return RedirectToAction("Register");
        }
        [HttpGet]
        public IActionResult Login() => View();
        
        [HttpPost]
        public IActionResult LoginPost()
        {
            Console.WriteLine("LoginPost has been activated");
            return RedirectToAction("Login");
        }
    }
}
