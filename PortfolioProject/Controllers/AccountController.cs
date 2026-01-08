using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioProject.Controllers;
using DataLayer.Models.ViewModels;
using DataLayer.Models;


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
                user.FirstName = registerViewModel.FirstName;
                user.LastName = registerViewModel.LastName;
                user.Email = registerViewModel.Email;

                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Code.StartsWith("Password"))
                        {
                            ModelState.AddModelError(
                                nameof(RegisterViewModel.Password),
                                error.Description
                            );
                        }
                        else if (error.Code == "DuplicateUserName")
                        {
                            ModelState.AddModelError(
                                nameof(RegisterViewModel.UserName),
                                error.Description
                            );
                        }
                    }
                }
            }
            return View(registerViewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel logInViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(logInViewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(logInViewModel.UserName, logInViewModel.Password,
                    isPersistent: logInViewModel.RememberMe, lockoutOnFailure: false);


            if (result.Succeeded)
            {

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Ogiltigt användarnamn eller lösenord.");

            return View(logInViewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        
    }

}