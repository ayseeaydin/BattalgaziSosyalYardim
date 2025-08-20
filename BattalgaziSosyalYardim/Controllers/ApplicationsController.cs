using BattalgaziSosyalYardim.Database;
using BattalgaziSosyalYardim.Entities;
using BattalgaziSosyalYardim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BattalgaziSosyalYardim.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(AppDbContext db, ILogger<ApplicationsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: /Applications/Create?programCode=bez-destegi
        [HttpGet]
        public IActionResult Create(string? programCode)
        {
            var model = new ApplicationCreateViewModel
            {
                ProgramCode = string.IsNullOrWhiteSpace(programCode) ? "bez-destegi" : programCode,
                ProgramTitle = "0-2 YAŞ BEBEK BEZİ DESTEĞİ BAŞVURU FORMU",
                MotherBirthDate = DateTime.Today
            };
            return View(model);
        }

        // POST: /Applications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            model.ProgramCode ??= "bez-destegi";

            // program var mı
            var program = await _db.AidPrograms
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == model.ProgramCode);

            if (program == null)
                ModelState.AddModelError(string.Empty, "Başvuru programı bulunamadı.");

            if (!model.MotherBirthDate.HasValue)
                ModelState.AddModelError(nameof(model.MotherBirthDate), "Anne Doğum Tarihi zorunludur.");
            else if (model.MotherBirthDate.Value.Date > DateTime.Today)
                ModelState.AddModelError(nameof(model.MotherBirthDate), "Anne Doğum Tarihi bugünden ileri olamaz.");

            if (!ModelState.IsValid)
            {
                // başlık boş gelirse doldur:
                model.ProgramTitle ??= "0-2 YAŞ BEBEK BEZİ DESTEĞİ BAŞVURU FORMU";
                return View(model);
            }

            // anne ad-soyad parçalama
            var parts = (model.MotherFullName ?? string.Empty)
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var first = parts.Length > 0 ? parts[0] : "";
            var last = parts.Length > 1 ? string.Join(' ', parts.Skip(1)) : "";

            var entity = new Application
            {
                AidProgramId = program!.Id,
                MotherNationalId = model.MotherNationalId.Trim(),
                MotherFirstName = first,
                MotherLastName = last,
                MotherBirthDate = model.MotherBirthDate!.Value.Date,
                PhoneNumber = model.PhoneNumber.Trim(),
                BabyNationalId = model.BabyNationalId.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            try
            {
                _db.Applications.Add(entity);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Başvuru kaydedilirken hata oluştu");
                ModelState.AddModelError(string.Empty, "Başvurunuz kaydedilirken bir hata oluştu. Lütfen bilgileri kontrol edip tekrar deneyiniz.");
                model.ProgramTitle ??= "0-2 YAŞ BEBEK BEZİ DESTEĞİ BAŞVURU FORMU";
                return View(model);
            }
            return RedirectToAction(nameof(Success));
        }

        // GET: /Applications/Success
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

    }
}
