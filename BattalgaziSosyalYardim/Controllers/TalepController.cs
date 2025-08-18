using System;
using System.Linq;
using System.Threading.Tasks;
using BattalgaziSosyalYardim.Database;
using BattalgaziSosyalYardim.Entities;
using BattalgaziSosyalYardim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BattalgaziSosyalYardim.Controllers
{
    [Route("Talep")]
    public class TalepController : Controller
    {
        private readonly AppDbContext _db;
        public TalepController(AppDbContext db) => _db = db;

        // ----------------- Yardımcılar -----------------
        private static string Digits(string? s) => new string((s ?? string.Empty).Where(char.IsDigit).ToArray());

        // Türkiye TCKN kontrol algoritması
        private static bool IsValidTckn(string? value)
        {
            var v = Digits(value);
            if (v.Length != 11 || v[0] == '0') return false;

            var d = v.Select(c => c - '0').ToArray();
            int oddSum = d[0] + d[2] + d[4] + d[6] + d[8];
            int evenSum = d[1] + d[3] + d[5] + d[7];

            int digit10 = ((oddSum * 7) - evenSum) % 10;
            if (digit10 != d[9]) return false;

            int digit11 = d.Take(10).Sum() % 10;
            return digit11 == d[10];
        }

        // 05xxxxxxxxx veya +90/0 öneklerini normalize et (DB'ye 05xxxxxxxxx olarak yazalım)
        private static string NormalizePhone(string phone)
        {
            var p = Digits(phone);
            if (p.StartsWith("90") && p.Length == 12 && p[2] == '5') p = p.Substring(2); // 90 5xxxxxxxxx
            if (p.Length == 10 && p.StartsWith("5")) p = "0" + p;                        // 5xxxxxxxxx -> 05xxxxxxxxx
            return p;
        }

        private static (string First, string Last) SplitFullName(string fullName)
        {
            var parts = (fullName ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return (string.Empty, string.Empty);
            var first = parts[0];
            var last = parts.Length > 1 ? string.Join(' ', parts.Skip(1)) : string.Empty;

            // Entity’deki HasMaxLength(50) ile uyumlu olsun
            if (first.Length > 50) first = first.Substring(0, 50);
            if (last.Length > 50) last = last.Substring(0, 50);

            return (first, last);
        }

        // ----------------- GET: /Talep -----------------
        [HttpGet("")]
        public IActionResult Create()
        {
            // Boş model ile formu aç
            var vm = new ApplicationCreateViewModel();
            return View("Create", vm);
        }

        // ----------------- POST: /Talep -----------------
        [HttpPost("")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            // 1) ModelState (DataAnnotations) doğrulaması
            if (!ModelState.IsValid)
                return View("Create", model);

            // 2) Ek sunucu doğrulamaları
            if (!IsValidTckn(model.MotherNationalId))
                ModelState.AddModelError(nameof(model.MotherNationalId), "Anne T.C. Kimlik No geçersiz.");

            if (!IsValidTckn(model.BabyNationalId))
                ModelState.AddModelError(nameof(model.BabyNationalId), "Bebek T.C. Kimlik No geçersiz.");

            if (model.MotherBirthDate.Date > DateTime.UtcNow.Date)
                ModelState.AddModelError(nameof(model.MotherBirthDate), "Doğum tarihi bugünden ileri olamaz.");

            var normalizedPhone = NormalizePhone(model.PhoneNumber);
            if (normalizedPhone.Length != 11 || !normalizedPhone.StartsWith("05"))
                ModelState.AddModelError(nameof(model.PhoneNumber), "Telefon 05XXXXXXXXX formatında olmalı.");

            if (!ModelState.IsValid)
                return View("Create", model);

            // 3) Yardım programını bul (ProgramCode ile, aktif olan)
            var program = await _db.AidPrograms.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == model.ProgramCode && p.IsActive);

            if (program == null)
            {
                ModelState.AddModelError("", "Aktif yardım programı bulunamadı.");
                return View("Create", model);
            }

            // 4) Anne ad/soyad ayrıştır
            var (firstName, lastName) = SplitFullName(model.MotherFullName);

            if (string.IsNullOrWhiteSpace(firstName))
                ModelState.AddModelError(nameof(model.MotherFullName), "Anne Ad Soyad boş olamaz.");
            if (!ModelState.IsValid)
                return View("Create", model);

            // 5) Aynı program + aynı bebek TCKN tekil olmalı (DB’de unique index de var)
            bool exists = await _db.Applications
                .AnyAsync(a => a.AidProgramId == program.Id && a.BabyNationalId == model.BabyNationalId);
            if (exists)
            {
                ModelState.AddModelError(nameof(model.BabyNationalId), "Bu bebek için aynı programda başvuru zaten mevcut.");
                return View("Create", model);
            }

            // 6) Kayıt nesnesi
            var entity = new Application
            {
                AidProgramId = program.Id,
                MotherNationalId = model.MotherNationalId,
                MotherFirstName = firstName,
                MotherLastName = lastName,
                MotherBirthDate = model.MotherBirthDate,
                PhoneNumber = normalizedPhone,
                BabyNationalId = model.BabyNationalId,
                Status = ApplicationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            // 7) DB'ye yaz
            _db.Applications.Add(entity);
            await _db.SaveChangesAsync();

            // 8) Başarılı → onay sayfasına yönlendir
            return RedirectToAction(nameof(Success), new { id = entity.Id });
        }

        // ----------------- GET: /Talep/Basarili/{id} -----------------
        [HttpGet("Basarili/{id:long}")]
        public async Task<IActionResult> Success(long id)
        {
            var app = await _db.Applications.AsNoTracking()
                .Include(a => a.AidProgram)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (app == null)
                return RedirectToAction(nameof(Create));

            ViewData["Ref"] = id;
            ViewData["Program"] = app.AidProgram?.Name ?? "Yardım Programı";
            return View(); // Views/Talep/Success.cshtml
        }
    }
}
