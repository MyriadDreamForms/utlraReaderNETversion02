using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using utlraReaderNETversion02.Models;
using utlraReaderNETversion02.Models.ViewModels;

namespace utlraReaderNETversion02.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class AdminController : Controller
    {
        // Giriş işlemi için sabit kullanıcı adları; bu örnek geliştirme aşamasında
        private const string AdminUsername = "admin";
        private const string ModeratorUsername = "moderator";

        // Giriş parolaları
        private const string AdminPassword = "1234";
        private const string ModeratorPassword = "mod123";

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

            string role = "";
            if (model.Username == AdminUsername && model.Password == AdminPassword)
                role = "Admin";
            else if (model.Username == ModeratorUsername && model.Password == ModeratorPassword)
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

            // Identity'nin default cookie şeması "Identity.Application" kullanılıyor.
            var identity = new ClaimsIdentity(claims, "Identity.Application");
            var principal = new ClaimsPrincipal(identity);

            // Oturum açtırma: burada şema olarak "Identity.Application" kullanılır.
            await HttpContext.SignInAsync("Identity.Application", principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return RedirectToAction("Dashboard", "Moderator");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            // Sadece admin için; moderatör için ModeratörController kullanılabilir.
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "webtoons");
            var directories = Directory.Exists(rootPath) ? Directory.GetDirectories(rootPath) : Array.Empty<string>();

            var webtoonList = new List<string>();
            int totalChapters = 0;

            foreach (var dir in directories)
            {
                string name = Path.GetFileName(dir);
                webtoonList.Add(name);
                totalChapters += Directory.GetDirectories(dir).Length;
            }

            ViewBag.TotalWebtoons = webtoonList.Count;
            ViewBag.TotalChapters = totalChapters;
            ViewBag.RecentWebtoons = webtoonList.OrderByDescending(n => n).Take(5).ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Identity'nin logout işlemi için Razor Pages kullanılan Identity Logout sayfası: 
            await HttpContext.SignOutAsync("Identity.Application");
            return RedirectToAction("Login");
        }
    }
}
