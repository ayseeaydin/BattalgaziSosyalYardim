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

        public AdminController(AppDbContext db) => _db = db;
        
        // GET: /Admin
        public async Task<IActionResult> Index()
        {
            ViewBag.Pending = await _db.Applications.CountAsync(a => a.Status == ApplicationStatus.Pending);
            ViewBag.Approved = await _db.Applications.CountAsync(a => a.Status == ApplicationStatus.Approved);
            ViewBag.Rejected = await _db.Applications.CountAsync(a => a.Status == ApplicationStatus.Rejected);
            return View();
        }
    }
}
