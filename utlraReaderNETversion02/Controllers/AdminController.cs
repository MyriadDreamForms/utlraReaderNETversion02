using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using utlraReaderNETversion02.Models.ViewModels;

namespace utlraReaderNETversion02.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private const string AdminUsername = "admin";
        private const string AdminPassword = "1234"; // Not: Sabit şifre kullanımını geliştirme aşamasında kullanın; prod'da hash’li saklayın

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

            // Sabit kullanıcı adı ve şifrenin kontrolü
            if (model.Username == AdminUsername && model.Password == "123456") // Not: Şifrenizi uyuşacak şekilde ayarlayın
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Eğer returnUrl varsa ve yerel ise ona yönlendir, yoksa Dashboard'a yönlendir
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Dashboard", "Admin");
            }

            ModelState.AddModelError("", "Kullanıcı adı veya şifre yanlış.");
            return View(model);
        }

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
