using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using pineapple_shopModel;

namespace testcore.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly pineapple_shopModel.pineapple_shopModel _shopModel;
        public AuthController(ILogger<AuthController> logger)
        {
            _shopModel = new pineapple_shopModel.pineapple_shopModel();
            _logger = logger;
        }
        public static string getHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed SHA256 = new SHA256Managed();
            byte[] hash = SHA256.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }

        public IActionResult Index()
        {
            return View("Login"); 
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        public async Task<IActionResult> Create([Bind("Email,Login,PasswordHash")] User user, string confirm_password)
        {
            if (ModelState.IsValid)
            {
                
                bool passwordsAreDifferent = (confirm_password != user.PasswordHash);
                int id;
                if (_shopModel.Users.Count() > 0)
                    id = _shopModel.Users.Max(m => m.Id) + 1;
                else
                    id = 0;
                user.PasswordHash = getHashSha256(user.PasswordHash);
                user.Id = id;
                user.RegDate = DateTime.Now;
                user.StatusId = 0;
                User matchByLogin = await _shopModel.Users.FirstOrDefaultAsync(u => u.Login == user.Login);
                User matchByEmail = await _shopModel.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                   
                if ((matchByLogin != null) || (matchByEmail!= null) || (passwordsAreDifferent))
                {
                    if (matchByLogin != null)
                        ModelState.AddModelError("Error", "This login already used. Please enter another");
                    if (matchByEmail != null)
                        ModelState.AddModelError("Error", "This email already used. Please enter another");
                    if (passwordsAreDifferent)
                        ModelState.AddModelError("Error", "Passwords are different. Please try again.");
                }
                else
                {
                    _shopModel.Add(user);
                    await _shopModel.SaveChangesAsync();
                    return RedirectToAction("Login", true);
                }
            }
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string input_login, string input_password)
        {
            if (ModelState.IsValid)
            {
                var current_user = await _shopModel.Users.FirstOrDefaultAsync(u => u.Login == input_login);
                bool isFound = (current_user != null);
                if (isFound)
                {
                    if (current_user.PasswordHash == getHashSha256(input_password))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimsIdentity.DefaultNameClaimType, current_user.Login)
                        };
                        ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                            ClaimsIdentity.DefaultRoleClaimType);
                        // setting auth cookies
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("Error", "Login or password are incorrect. Please try again.");
            return View("Login");
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
