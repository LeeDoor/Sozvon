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
            _users.Add(user);

            return RedirectToAction("Login", new UserCredential { 
                Login = user.Login, Password = user.Password });
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
