using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioProject.Data;
using PortfolioProject.Models;

namespace PortfolioProject.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DatabaseContext _dbContext;
        private readonly User _loggedIn;

        public ProfileController(DatabaseContext dbContext, UserManager<User> userManager) {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit() {
            var user = await _userManager.GetUserAsync(User);

            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Edit() { 
            //For saving later
        //}

    }
}
