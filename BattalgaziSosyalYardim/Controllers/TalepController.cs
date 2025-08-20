using BattalgaziSosyalYardim.Database;
using BattalgaziSosyalYardim.Entities;
using BattalgaziSosyalYardim.Models;
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
        [HttpGet]
        public IActionResult Create()
        {
            // Doğrudan Applications klasöründeki view’i çağırıyoruz
            return View("~/Views/Applications/Create.cshtml", new ApplicationCreateViewModel());
        }

        // POST: /Talep/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateViewModel model)
        {
            // Program kodu boş geldiyse varsayılanı ver
            model.ProgramCode ??= "bez-destegi";

            // Program var mı?
            var program = await _db.AidPrograms
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == model.ProgramCode);

            if (program == null)
            {
                ModelState.AddModelError(string.Empty, "Başvuru programı bulunamadı.");
            }

            // Doğum tarihi nullable olduğundan ayrıca kontrol et
            if (!model.MotherBirthDate.HasValue)
            {
                ModelState.AddModelError(nameof(model.MotherBirthDate), "Anne Doğum Tarihi zorunludur.");
            }
            else if (model.MotherBirthDate.Value.Date > DateTime.Today)
            {
                ModelState.AddModelError(nameof(model.MotherBirthDate), "Anne Doğum Tarihi bugünden ileri olamaz.");
            }

            if (!ModelState.IsValid)
            {
                // Hataları aynı sayfada göster
                return View("~/Views/Applications/Create.cshtml", model);
            }

            // Anne ad soyadı parçalama
            var parts = (model.MotherFullName ?? string.Empty)
                        .Trim()
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var first = parts.Length > 0 ? parts[0] : "";
            var last = parts.Length > 1 ? string.Join(' ', parts.Skip(1)) : "";

            // Kayıt entity’si
            var entity = new Application
            {
                AidProgramId = program!.Id,
                MotherNationalId = model.MotherNationalId.Trim(),
                MotherFirstName = first,
                MotherLastName = last,
                MotherBirthDate = model.MotherBirthDate!.Value.Date, // <-- önemli kısım
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
