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

        // GET: /Admin
        // NOT: Index view'iniz IEnumerable<Application> bekliyorsa model olarak liste gönderiyoruz.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // üstte kutucuklar için sayılar (kullanıyorsanız)
            ViewBag.Pending = await _db.Applications.CountAsync(a => a.Status == ApplicationStatus.Pending);
            ViewBag.Approved = await _db.Applications.CountAsync(a => a.Status == ApplicationStatus.Approved);
            ViewBag.Rejected = await _db.Applications.CountAsync(a => a.Status == ApplicationStatus.Rejected);

            // liste (DataTables vb. için)
            var apps = await _db.Applications
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                .ToListAsync();

            return View(apps); // <-- NULL REFERANS HATASINI ÇÖZER
        }

        // GET: /Admin/Applications?status=pending|approved|rejected
        // İsterseniz /Admin/Applications sayfasında da filtreli liste gösterebilirsiniz.
        [HttpGet]
        public async Task<IActionResult> Applications(string? status = null)
        {
            var q = _db.Applications
                .AsNoTracking()
                .OrderByDescending(a => a.Id)
                .AsQueryable();

            string st = "all";
            if (!string.IsNullOrWhiteSpace(status))
            {
                st = status.ToLowerInvariant();
                q = st switch
                {
                    "approved" => q.Where(a => a.Status == ApplicationStatus.Approved),
                    "rejected" => q.Where(a => a.Status == ApplicationStatus.Rejected),
                    "pending" => q.Where(a => a.Status == ApplicationStatus.Pending),
                    _ => q
                };
            }

            ViewBag.Status = st;
            var list = await q.ToListAsync();
            return View(list);
        }
    }
}
