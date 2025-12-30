using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioProject.Data;
using PortfolioProject.Models;

namespace PortfolioProject.Controllers
{
    public class ProjectController : Controller
    {
        private readonly DatabaseContext _context;

        public ProjectController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Projects()
        {
            bool isLoggedIn = User.Identity?.IsAuthenticated ?? false;

            var projects = _context.Projects
                .Include(p => p.Users)
                .Select(p => new ProjectViewModel
                {
                    Project = p,
                    VisibleUsers = isLoggedIn ? p.Users : p.Users.Where(u => !u.IsPrivate)
                })
                .ToList();

            return View(projects);
        }

        //public IActionResult Edit()
        //{
            
        //}
    }
}
