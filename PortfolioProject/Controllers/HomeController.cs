using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioProject.Data;
using PortfolioProject.Models;

namespace PortfolioProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _dbContext;

        public HomeController(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var cvList = new List<Cv>();
            if (!User.Identity.IsAuthenticated) {
                //lägg till att ej visa dekativerade kontons cv
                cvList = await _dbContext.Cvs.Where(cv =>cv.User.IsPrivate == false).ToListAsync();
            }
            else
            {
                cvList = await _dbContext.Cvs.ToListAsync();
            }

            //hämta senaste projekt

            var mv = new HomeViewModel
            {
                Cvs = cvList
                //spara senaste projekten
            };

                return View(mv);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
