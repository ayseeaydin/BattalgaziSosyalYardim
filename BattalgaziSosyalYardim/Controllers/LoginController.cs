using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BattalgaziSosyalYardim.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace BattalgaziSosyalYardim.Controllers
{
    public class LoginController : Controller
    {
        private readonly AdminAuthOptions _opts;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IOptions<AdminAuthOptions> opts, ILogger<LoginController> logger)
        {
            _opts = opts.Value;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = string.IsNullOrWhiteSpace(returnUrl) ? "/Admin" : returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            returnUrl??= "/Admin";

            if (!ModelState.IsValid)
                return View(model);

            var ok = string.Equals(model.Username?.Trim(), _opts.Username?.Trim(), StringComparison.Ordinal)
                && string.Equals(model.Password, _opts.Password, StringComparison.Ordinal);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username),
                new Claim(ClaimTypes.Role,"Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);


            _logger.LogInformation("Admin giriş yaptı{user}", model.Username);
            return LocalRedirect(returnUrl);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Denied() => View();
    }
}
