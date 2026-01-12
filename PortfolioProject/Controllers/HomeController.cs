using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLayer.Data;
using DataLayer.Models.ViewModels;
using DataLayer.Models;

namespace PortfolioProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<User> _userManager;

        public HomeController(DatabaseContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            bool isAuthenticated = User.Identity.IsAuthenticated;


            //om användaren är inloggad hämtas de cvn med aktiv användare
            //om användaren inte är inloggad hämtas de cvn som har icke privata och aktiva användare
            var cvList = isAuthenticated ? 
                await _dbContext.Cvs
                    .Where(cv => cv.User.IsActive == true)
                    .ToListAsync() : 
                await _dbContext.Cvs
                    .Where(cv => cv.User.IsPrivate == false 
                        && cv.User.IsActive == true)
                    .ToListAsync();

            var projectList = await _dbContext.Projects
                    .Include(p => p.Users)
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(3)
                    .ToListAsync();

            foreach(var project in projectList)
            {
                project.Users = isAuthenticated ?
                    project.Users.Where(u => u.Id != project.OwnerId && u.IsActive).ToList() :
                    project.Users.Where(u => u.Id != project.OwnerId && !u.IsPrivate && u.IsActive).ToList();
            }
            
            var skills = await _dbContext.Skills.ToListAsync();

            var mv = new HomeViewModel
            {
                Cvs = cvList,
                Projects = projectList,
                Skills = skills
            };

            return View(mv);
        }

        [HttpGet]
        public IActionResult Search(string name, string skill)
        {
            var query = _dbContext.Cvs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(cv => cv.User.FirstName.ToLower().StartsWith(name));
            }
            if (!string.IsNullOrWhiteSpace(skill))
            {
                query = query.Where(cv => cv.Skills.Any(s => s.Name.Equals(skill)));
            }
            

            if (User.Identity.IsAuthenticated)
            {
                query = query.Where(cv => cv.User.IsActive == true);
            }
            else
            {
                query = query.Where(cv => cv.User.IsPrivate == false && cv.User.IsActive == true);
            }
            
            var result = query.ToList();

            return PartialView("CvCarouselPartial", result);
        }



        [HttpPost]
        public async Task<IActionResult> JoinProject(Guid pid, string returnUrl)
        {
            var userId = _userManager.GetUserId(User);

            User? user = await _dbContext.Users
                .Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.Id == userId);

            Project? project = await _dbContext.Projects
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == pid);

            if(user == null || project == null)
            {
                return NotFound();
            }

            if (!user.Projects.Any(p => p.Id == pid))
            {
                user.Projects.Add(project);
                await _dbContext.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        } 

    }
}
