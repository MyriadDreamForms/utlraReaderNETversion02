using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using utlraReaderNETversion02.Models.ViewModels;

namespace utlraReaderNETversion02.Controllers
{
    [Authorize(Roles = "Moderator")]
    public class ModeratorController : Controller
    {
        private const string ModUsername = "mod";
        private const string ModPassword = "1234";

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(); // Views/Moderator/Login.cshtml
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Username == ModUsername && model.Password == ModPassword)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "Moderator")
                };

                var identity = new ClaimsIdentity(claims, "Identity.Application");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Identity.Application", principal);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Dashboard", "Moderator");
            }

            ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre yanlış.");
            return View(model);
        }

        public IActionResult Dashboard()
        {
            // Moderatör paneli; burada istatistikler veya onay işlemleri eklenebilir.
            // Controller'dan ViewBag ile gönderilen veriler kullanılabilir.
            return View(); // Views/Moderator/Dashboard.cshtml
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Identity.Application");
            return RedirectToAction("Login");
        }
    }
}
