using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioProject.Data;
using PortfolioProject.Models;
using System.Threading.Tasks;

namespace PortfolioProject.Controllers
{
    public class ProjectController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<User> _userManager;

        public ProjectController(DatabaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Projects()
        {
            bool isLoggedIn = User.Identity?.IsAuthenticated ?? false;

            var projects = _context.Projects
                .Select(p => new ProjectViewModel
                {
                    Project = p,
                    VisibleUsers = isLoggedIn ? p.Users : p.Users.Where(u => !u.IsPrivate)
                })
                .ToList();

            return View(projects);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProjectViewModel
            {
                Project = new Project()
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(ProjectViewModel projectVM)
        {
            if(!ModelState.IsValid)
            {
                return View(projectVM);
            }

            var userId = _userManager.GetUserId(User);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if(user == null)
                return Unauthorized();

            Project newProject = new Project
            {
                Title = projectVM.Project.Title,
                Description = projectVM.Project.Description,
                CreatedDate = DateTime.UtcNow,
                OwnerId = userId,
                Users = new List<User> { user }
            };

            await _context.Projects.AddAsync(newProject);
            await _context.SaveChangesAsync();

            return RedirectToAction("Projects");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> JoinProject(Guid pid, string returnUrl)
        {
            var userId = _userManager.GetUserId(User);

            User? user = await _context.Users
                .Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.Id == userId);

            Project? project = await _context.Projects
                .Include(p => p.Users)
                .FirstOrDefaultAsync(p => p.Id == pid);

            if(user == null || project == null)
                return NotFound();

            if(!user.Projects.Any(p => p.Id == pid))
            {
                user.Projects.Add(project);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Projects");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var userId = _userManager.GetUserId(User);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id);

            if(project == null)
                return NotFound();

            if(project.OwnerId == null || project.OwnerId != userId)
                return Forbid();

            var projectVM = new ProjectViewModel
            {
                Project = project
            };

            return View(projectVM);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(ProjectViewModel projectVM)
        {
            var userId = _userManager.GetUserId(User);

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectVM.Project.Id);

            if (project == null)
                return NotFound();

            if(project.OwnerId == null || project.OwnerId != userId)
                return Forbid();

            project.Title = projectVM.Project.Title;
            project.Description = projectVM.Project.Description;
            await _context.SaveChangesAsync();

            return RedirectToAction("Projects");
        }

        public async Task<IActionResult> Details(Guid id)
        {
            bool isLoggedIn = User.Identity?.IsAuthenticated ?? false;

            var project = await _context.Projects
                .Include(p => p.Users)
                .Where(p => p.Id == id)
                .Select(p => new ProjectViewModel
                {
                    Project = p,
                    VisibleUsers = isLoggedIn ? p.Users : p.Users.Where(u => !u.IsPrivate)
                })
                .FirstOrDefaultAsync();

            if(project == null)
                return NotFound();

            return View(project);
        }
    }
}
