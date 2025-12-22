using Microsoft.AspNetCore.Mvc;

namespace PortfolioProject.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
