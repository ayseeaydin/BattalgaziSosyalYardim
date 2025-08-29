using BattalgaziSosyalYardim.Database;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BattalgaziSosyalYardim.Models;
using BattalgaziSosyalYardim.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace BattalgaziSosyalYardim.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AppDbContext db, ILogger<LoginController> logger)
        {
            _db = db;
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
            returnUrl ??= "/Admin";

            if (!ModelState.IsValid)
                return View(model);

            var tckn = (model.NationalId ?? "").Trim();
            var user = await _db.AdminUsers.AsNoTracking().FirstOrDefaultAsync(u => u.NationalId == tckn);

            if (user == null || !user.IsActive || !PasswordHasher.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "T.C. Kimlik No veya şifre hatalı.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("national_id", user.NationalId),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = false } 
            );

            var tracked = await _db.AdminUsers.FirstAsync(u => u.Id == user.Id);
            tracked.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            _logger.LogInformation("Admin giriş yaptı: {tckn}", user.NationalId);
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
