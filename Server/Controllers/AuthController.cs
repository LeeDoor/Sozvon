using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using Server.Models.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System;
using System.Data;

namespace SignalRApp
{
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            
            await Clients.All.SendAsync("Receive", message);
        }
    public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} вошел в чат");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} покинул в чат");
            await base.OnDisconnectedAsync(exception);
        }
    }
}


namespace Server.Controllers
{
    public class AuthController : Controller
    {
        private List<User> _users = new();
        [HttpGet]
        public IActionResult Register() => View();
        [HttpPost]
        public IActionResult RegisterPost([Required] User user)
        {
            _users.Add(new User()
            {
                Login = user.Login,
                Name = user.Name,
                Password = user.Password
            });
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Login() => View();
        
        [HttpPost]
        public async Task<IActionResult> LoginPost(string? returnUrl, [Required] User user)
        {
            if (user is null) return Unauthorized();
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Login)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            if (returnUrl is not null)
                return LocalRedirect(returnUrl);
            return Redirect("/");
        }
    }
}
