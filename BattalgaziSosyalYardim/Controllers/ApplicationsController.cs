using Microsoft.AspNetCore.Mvc;

namespace BattalgaziSosyalYardim.Controllers
{
    public class ApplicationsController : Controller
    {
        // GET: /Applications/Create?programCode=bez-destegi
        [HttpGet]
        public IActionResult Create(string? programCode)
        {
            // (İstersen Create.cshtml’de kullanırsın)
            ViewData["ProgramCode"] = programCode ?? "bez-destegi";
            ViewData["ProgramTitle"] = "0-2 YAŞ BEBEK BEZİ DESTEĞİ BAŞVURU FORMU";
            return View();
        }
    }
}
