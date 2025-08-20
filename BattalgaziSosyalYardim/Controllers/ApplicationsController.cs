using BattalgaziSosyalYardim.Models;
using Microsoft.AspNetCore.Mvc;

namespace BattalgaziSosyalYardim.Controllers
{
    public class ApplicationsController : Controller
    {
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

            return View(model); // <-- ÖNEMLİ
        }

    }
}
