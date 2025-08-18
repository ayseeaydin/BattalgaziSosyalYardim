using BattalgaziSosyalYardim.Database;
using BattalgaziSosyalYardim.Entities;
using BattalgaziSosyalYardim.Models;
// using BattalgaziSosyalYardim.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BattalgaziSosyalYardim.Controllers
{
    public class TalepController : Controller
    {
        private readonly AppDbContext _db;

        public TalepController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /Talep/Create
        public IActionResult Create()
        {
            // Doğrudan Applications klasöründeki view’i çağırıyoruz
            return View("~/Views/Applications/Create.cshtml");
        }

        // POST: /Talep/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            // AidProgram kontrolü
            var program = await _db.AidPrograms
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == model.ProgramCode);

            if (program == null)
            {
                ModelState.AddModelError(string.Empty, "Başvuru programı bulunamadı.");
            }

            if (!ModelState.IsValid)
            {
                // Eğer formda hata varsa tekrar Applications/Create.cshtml view’i yükle
                return View("~/Views/Applications/Create.cshtml", model);
            }

            // Anne adı soyadı parçalama
            var names = (model.MotherFullName ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var first = names.Length > 0 ? names[0] : "";
            var last = names.Length > 1 ? string.Join(' ', names.Skip(1)) : "";

            // Entity oluşturma
            var entity = new Application
            {
                AidProgramId = program!.Id,
                MotherNationalId = model.MotherNationalId.Trim(),
                MotherFirstName = first,
                MotherLastName = last,
                MotherBirthDate = model.MotherBirthDate.Date,
                PhoneNumber = model.PhoneNumber.Trim(),
                BabyNationalId = model.BabyNationalId.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            _db.Applications.Add(entity);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
