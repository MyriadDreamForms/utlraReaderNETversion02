using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using utlraReaderNETversion02.Models.ViewModels;

namespace utlraReaderNETversion02.Controllers
{
    [Authorize(Roles = "Admin,Moderator")] // Artık her iki rol de girebilir
    public class AdminController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Rol belirleme
            string role = "";
            if (model.Username == "admin" && model.Password == "1234")
                role = "Admin";
            else if (model.Username == "moderator" && model.Password == "mod123")
                role = "Moderator";
            else
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Rolüne göre yönlendirme
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Dashboard", "Moderator");
        }

        [Authorize(Roles = "Admin")] // Dashboard sadece admin'e özel
        public IActionResult Dashboard()
        {
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "webtoons");
            var directories = Directory.Exists(rootPath) ? Directory.GetDirectories(rootPath) : new string[0];

            var webtoonList = new List<string>();
            int totalChapters = 0;

            foreach (var dir in directories)
            {
                string name = Path.GetFileName(dir);
                webtoonList.Add(name);

                var chapterDirs = Directory.GetDirectories(dir);
                totalChapters += chapterDirs.Length;
            }

            ViewBag.TotalWebtoons = webtoonList.Count;
            ViewBag.TotalChapters = totalChapters;
            ViewBag.RecentWebtoons = webtoonList.OrderByDescending(n => n).Take(5).ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
