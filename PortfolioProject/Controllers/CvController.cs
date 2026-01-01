using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioProject.Data;
using PortfolioProject.Models;
using System.Threading.Tasks;

namespace PortfolioProject.Controllers
{
    [Route("cv")]
    public class CvController : Controller
    {
        private readonly DatabaseContext _db;

        public CvController(DatabaseContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Details(string username)
        {
            var isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            var user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }

            if (user.IsPrivate && !isLoggedIn)
            {
                return View("Private");
            }

            Cv? cv = await _db.Cvs.AsNoTracking()
                .Include(c => c.Skills)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cv == null)
            {
                return NotFound();
            }

            CvDetailsViewModel cvVm = new CvDetailsViewModel
            {
                UserName = user.UserName,
                FullName = (user.FirstName + " " + user.LastName).Trim(),
                ProfileImagePath = user.ProfileImageUrl,
                IsPrivate = user.IsPrivate,

                ViewCount = cv.ViewCount,
                Title = cv.Title,
                Summary = cv.Summary,
                Skills = cv.Skills,
                Educations = cv.Educations,
                Experiences = cv.Experiences

            };


            return View("Details", cvVm);
        }
    }
}
