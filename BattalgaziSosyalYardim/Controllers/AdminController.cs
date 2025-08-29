using Microsoft.AspNetCore.Mvc;
using BattalgaziSosyalYardim.Database;
using BattalgaziSosyalYardim.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BattalgaziSosyalYardim.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext db, ILogger<AdminController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var apps = await _db.Applications
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                .ToListAsync();

            return View(apps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(long id, string? reason)
        {
            var app = await _db.Applications.FirstOrDefaultAsync(a => a.Id == id);
            if (app == null)
            {
                TempData["err"] = "Başvuru bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            if (app.Status != ApplicationStatus.Pending)
            {
                TempData["err"] = "Bu başvuru için karar zaten verilmiş.";
                return RedirectToAction(nameof(Index));
            }

            app.Status = ApplicationStatus.Approved;
            app.Notes = (reason ?? string.Empty).Trim();
            app.RejectionReason = null;
            app.DecisionUserId = User.Identity?.Name ?? "admin";
            app.DecisionDate = DateTime.UtcNow;
            app.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            TempData["ok"] = "Başvuru onaylandı.";
            _logger.LogInformation("Application {Id} approved by {User}", id, app.DecisionUserId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(long id)
        {
            var app = await _db.Applications.FirstOrDefaultAsync(a => a.Id == id);
            if (app == null)
            {
                TempData["err"] = "Başvuru bulunamadı.";
                return RedirectToAction(nameof(Index));
            }
            if (app.Status != ApplicationStatus.Pending)
            {
                TempData["err"] = "Bu başvuru için karar zaten verilmiş.";
                return RedirectToAction(nameof(Index));
            }

            app.Status = ApplicationStatus.Rejected;
            app.DecisionUserId = User.Identity?.Name ?? "admin";
            app.DecisionDate = DateTime.UtcNow;
            app.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            TempData["ok"] = "Başvuru reddedildi.";
            _logger.LogInformation("Application {Id} rejected by {User}", id, app.DecisionUserId);
            return RedirectToAction(nameof(Index));
        }
    }
}
