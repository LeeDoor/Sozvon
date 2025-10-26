using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System.ComponentModel.DataAnnotations;
using Server.Models.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Server.Models.Data.Services;
using Microsoft.AspNetCore.Identity;
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
            return RedirectToAction("Login", new UserCredential { Login = user.Login, Password = user.Password });
        }
        [HttpGet]
        public IActionResult Login(UserCredential userCredential)
        {
            if (userCredential.Login is not null) return View(userCredential);
            if (User.Identity is not null) return View(new UserCredential { Login = User.Identity.Name, Password = "" });
            return View();
        }

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

            return Redirect("/Room/Index");
        }
    }
}

