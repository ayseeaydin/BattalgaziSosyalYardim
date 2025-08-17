using Microsoft.AspNetCore.Mvc;

namespace BattalgaziSosyalYardim.Controllers
{
    public class ApplicationsController : Controller
    {
        [HttpGet]
        public IActionResult Create(string? programCode = null)
        {
            // ekranda başlık vs. için istersen kullanırsın
            ViewData["ProgramTitle"] = programCode == "bez-destegi"
                ? "Bebek Bezi Desteği"
                : "Başvuru Formu";

            return View();
        }

        // İleride POST action ekleyeceğiz (şimdilik görünüm odaklıyız)
    }
}
