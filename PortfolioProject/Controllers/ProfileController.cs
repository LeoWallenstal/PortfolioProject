using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLayer.Data;
using DataLayer.Models.ViewModels;
using DataLayer.Models;

namespace PortfolioProject.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DatabaseContext _dbContext;

        public ProfileController(DatabaseContext dbContext, UserManager<User> userManager) {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            return View(new UserViewModel(user));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(UserViewModel userModel) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var viewModel = new UserViewModel(user); // create ViewModel from DB entity
            return View(viewModel);
        }

        //[HttpPost]
        //public async Task<IActionResult> Edit(User editedUser, string action) {
        //    if (action == "cancel") {
        //        return RedirectToAction("Index");
        //    }

        //    var user = await _userManager.GetUserAsync(User);

        //    if (!ModelState.IsValid)
        //    {
        //        return View(user);
        //    }

        //    //Validera osv, sen spara

        //}

    }
}
