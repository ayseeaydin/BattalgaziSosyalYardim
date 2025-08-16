using System.Threading.Tasks;
using BattalgaziSosyalYardim.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BattalgaziSosyalYardim.Controllers
{
    public class AidProgramController : Controller
    {
        private readonly AppDbContext _db;
        public AidProgramController(AppDbContext db) => _db = db;

        // GET: /AidProgram
        // Amaç; seed edilen yardım programlarını listelemek
        public async Task<IActionResult> Index()
        {
            var items = await _db.AidPrograms
                .AsNoTracking()
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.Name)
                .ToListAsync();

            return View(items);
        }
    }
}
