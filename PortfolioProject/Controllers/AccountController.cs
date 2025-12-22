using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioProject.Controllers;
using PortfolioProject.Models;

namespace PortfolioProject.Controllers

{
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            return View(loginViewModel);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                
                User user = new User();
                user.UserName = registerViewModel.UserName;

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(registerViewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel logInViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(logInViewModel.UserName, logInViewModel.Password, 
                    isPersistent:logInViewModel.RememberMe, lockoutOnFailure:false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.IsLockedOut)
                    ModelState.AddModelError("", "This account is locked.");

                else if (result.IsNotAllowed)
                    ModelState.AddModelError("", "You are not allowed to log in (email not confirmed?).");

                else if (result.RequiresTwoFactor)
                    ModelState.AddModelError("", "Two-factor authentication is required.");

                else
                    ModelState.AddModelError("", "Invalid username or password.");
            }

            return View(logInViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        
    }

}