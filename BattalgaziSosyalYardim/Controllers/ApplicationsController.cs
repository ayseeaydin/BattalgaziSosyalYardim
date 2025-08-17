using System;
using System.Linq;
using System.Threading.Tasks;
using BattalgaziSosyalYardim.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BattalgaziSosyalYardim.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly AppDbContext _db;
        public ApplicationsController(AppDbContext db) => _db = db;

        // GET: /Applications/Create?programCode=bez-destegi
        public async Task<IActionResult> Create(string? programCode)
        {
            // İsteğe bağlı: program adını başlığa yazmak istersen:
            if (!string.IsNullOrWhiteSpace(programCode))
            {
                var program = await _db.AidPrograms
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Code == programCode);

                ViewData["ProgramTitle"] = program?.Name ?? "Bebek Bezi Desteği";
            }
            else
            {
                ViewData["ProgramTitle"] = "Bebek Bezi Desteği";
            }

            return View();
        }
    }
}
