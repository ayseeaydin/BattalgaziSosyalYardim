using BattalgaziSosyalYardim.Database;
using BattalgaziSosyalYardim.Entities;
using BattalgaziSosyalYardim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql; // <-- eklendi

namespace BattalgaziSosyalYardim.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<ApplicationsController> _logger;
        private readonly IWebHostEnvironment _env;

        public ApplicationsController(AppDbContext db, ILogger<ApplicationsController> logger, IWebHostEnvironment env)
        {
            _db = db;
            _logger = logger;
            _env = env;
        }

        // GET: /Applications/Create?programCode=bez-destegi
        [HttpGet]
        public IActionResult Create(string? programCode)
        {
            var model = new ApplicationCreateViewModel
            {
                ProgramCode = string.IsNullOrWhiteSpace(programCode) ? "bez-destegi" : programCode,
                ProgramTitle = "0-2 YAŞ BEBEK BEZİ DESTEĞİ BAŞVURU FORMU",
                // İstersen boş bırakabilirsin; uyarı istemiyorsan setli kalabilir:
                // MotherBirthDate = DateTime.Today
            };

            return View(model);
        }

        // POST: /Applications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            model.ProgramCode ??= "bez-destegi";

            // Program var mı?
            var program = await _db.AidPrograms
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == model.ProgramCode);

            if (program == null)
                ModelState.AddModelError(string.Empty, "Başvuru programı bulunamadı.");

            // Doğum tarihi temel kontrol (ileriye tarih verilmesin)
            if (model.MotherBirthDate.HasValue && model.MotherBirthDate.Value.Date > DateTime.Today)
                ModelState.AddModelError(nameof(model.MotherBirthDate), "Anne Doğum Tarihi bugünden ileri olamaz.");

            if (!ModelState.IsValid)
            {
                model.ProgramTitle ??= "0-2 YAŞ BEBEK BEZİ DESTEĞİ BAŞVURU FORMU";
                return View(model);
            }

            // Anne ad/soyad parçalama
            var parts = (model.MotherFullName ?? string.Empty)
                        .Trim()
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var first = parts.Length > 0 ? parts[0] : "";
            var last = parts.Length > 1 ? string.Join(' ', parts.Skip(1)) : "";

            var entity = new Application
            {
                AidProgramId = program!.Id,
                MotherNationalId = (model.MotherNationalId ?? "").Trim(),
                MotherFirstName = first,
                MotherLastName = last,
                MotherBirthDate = model.MotherBirthDate?.Date ?? DateTime.Today,
                PhoneNumber = (model.PhoneNumber ?? "").Trim(),
                BabyNationalId = (model.BabyNationalId ?? "").Trim(),
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
                _logger.LogError(ex, "Başvuru kaydı sırasında hata.");

                // Sağlayıcı bazlı hata (PostgreSQL) ise ayrıştır
                if (ex.InnerException is PostgresException pg)
                {
                    // UNIQUE ihlali: (AidProgramId, BabyNationalId)
                    if (pg.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        ModelState.AddModelError(nameof(model.BabyNationalId),
                            "Bu bebek için bu programda daha önce başvuru yapılmış.");
                    }
                    // CHECK ihlalleri (regex)
                    else if (pg.SqlState == PostgresErrorCodes.CheckViolation)
                    {
                        switch (pg.ConstraintName)
                        {
                            case "ck_phone_number":
                                ModelState.AddModelError(nameof(model.PhoneNumber), "Telefon formatı geçersiz.");
                                break;
                            case "ck_mother_national_id":
                                ModelState.AddModelError(nameof(model.MotherNationalId), "Anne T.C. Kimlik No 11 haneli olmalıdır.");
                                break;
                            case "ck_baby_national_id":
                                ModelState.AddModelError(nameof(model.BabyNationalId), "Bebek T.C. Kimlik No 11 haneli olmalıdır.");
                                break;
                            default:
                                ModelState.AddModelError(string.Empty, "Form doğrulamasında bir hata oluştu.");
                                break;
                        }
                    }
                    else
                    {
                        // Diğer Postgres hataları
                        ModelState.AddModelError(string.Empty, "Başvurunuz kaydedilirken beklenmeyen bir veritabanı hatası oluştu.");
                    }
                }
                else
                {
                    // Sağlayıcı dışı bir DbUpdateException ise
                    ModelState.AddModelError(string.Empty, "Başvurunuz kaydedilemedi.");
                }

                // DEV ortamında ayrıntıyı da kullanıcıya göster (sadece debug amaçlı)
                if (_env.IsDevelopment())
                {
                    ModelState.AddModelError(string.Empty, $"[DEBUG] {ex.GetType().Name}: {ex.InnerException?.Message ?? ex.Message}");
                }

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
