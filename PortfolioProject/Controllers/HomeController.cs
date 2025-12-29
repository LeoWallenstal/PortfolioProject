using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioProject.Data;
using PortfolioProject.Models;

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
            var cvList = new List<Cv>();
            var projectList = new List<Project>();
            var skills = new List<Skill>();

            if (User.Identity.IsAuthenticated) {
                cvList = await _dbContext.Cvs.ToListAsync();
            }
            else
            {
                //lägg till att ej visa dekativerade kontons cv
                cvList = await _dbContext.Cvs.Where(cv => cv.User.IsPrivate == false).ToListAsync();
            }

            projectList = await _dbContext.Projects.Include(p => p.Users).OrderByDescending(p => p.CreatedDate).ToListAsync();
            skills = await _dbContext.Skills.ToListAsync();

            var mv = new HomeViewModel
            {
                Cvs = cvList,
                Projects = projectList,
                Skills = skills
            };

            return View(mv);
        }

        [HttpPost]
        public async Task<IActionResult> JoinProject(Guid pid)
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

            return RedirectToAction("Index");
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
