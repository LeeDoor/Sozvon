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
using Microsoft.AspNetCore.Authorization;
using Server.Models.Data.Services;

namespace SignalRApp
{
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            await Clients.All.SendAsync("Receive", message, Context.User.Identity.Name);
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
    public class ChatController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();
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
        public async Task<IActionResult> RegisterPost(UserService userService, [FromBody][Required] User user)
        {
            await userService.CreateUserAsync(user);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Login() => View();
        
        [HttpPost]
        public async Task<IActionResult> LoginPost(
            UserService userService, string? returnUrl, 
            [Required][FromBody] UserCredential userCredential)
        {
            if (!await userService.ValidateUserAsync(userCredential))
                return Unauthorized();
            if (userCredential is null) return Unauthorized();
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, userCredential.Login)
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
