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
        public ApplicationsController(AppDbContext db) => _db = db;

        // GET: /Applications/Create?programCode=bez-destegi
        [HttpGet]
        public async Task<IActionResult> Create(string programCode = "bez-destegi")
        {
            var program = await _db.AidPrograms.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == programCode && p.IsActive);

            var vm = new ApplicationCreateViewModel
            {
                ProgramCode = programCode,
                ProgramTitle = "0-2 YAŞ BEBEK BEZİ DESTEĞİ BAŞVURU FORMU",
                MotherBirthDate = DateTime.Today 
            };

            // program bulunamazsa da form açılır; başlık sabit kalır
            if (program is null) ViewBag.ProgramNotFound = true;

            return View(vm);
        }

        // POST: /Applications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // Anne ad/soyadı parçalama (basit: son kelime soyad)
            var parts = (vm.MotherFullName ?? "").Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = parts.Length > 1 ? string.Join(' ', parts.Take(parts.Length - 1)) : vm.MotherFullName.Trim();
            var lastName = parts.Length > 1 ? parts.Last() : "";

            // Programı bul
            var program = await _db.AidPrograms.FirstOrDefaultAsync(p => p.Code == vm.ProgramCode);
            if (program is null)
            {
                ModelState.AddModelError("", "Yardım programı bulunamadı.");
                return View(vm);
            }

            var app = new Application
            {
                AidProgramId = program.Id,
                MotherNationalId = vm.MotherNationalId,
                MotherFirstName = firstName,
                MotherLastName = lastName,
                MotherBirthDate = vm.MotherBirthDate,
                PhoneNumber = vm.PhoneNumber,
                BabyNationalId = vm.BabyNationalId,
                Status = ApplicationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            _db.Applications.Add(app);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Benzersizlik (aynı program + aynı bebek) ihlali için kullanıcıya mesaj
                ModelState.AddModelError("", "Bu bebek için aynı programda mevcut bir başvuru bulunuyor.");
                return View(vm);
            }

            TempData["ok"] = "Başvurunuz alınmıştır.";
            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success()
        {
            ViewData["Title"] = "Başvuru Alındı";
            return View();
        }
    }
}
