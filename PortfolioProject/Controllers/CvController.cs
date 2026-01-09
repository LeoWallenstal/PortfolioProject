using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLayer.Models.ViewModels;
using DataLayer.Models;
using DataLayer.Data;
using System.Threading.Tasks;

namespace PortfolioProject.Controllers
{
    [Route("cv")]
    public class CvController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly UserManager<User> _userManager;

        public CvController(DatabaseContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Details(string username)
        {
            var isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;
            var viewerId = _userManager.GetUserId(User);

            var cvUser = await _db.Users.Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.UserName == username);
            if (cvUser == null)
            {
                return NotFound();
            }

            if (cvUser.IsPrivate && !isLoggedIn)
            {
                return View("Private");
            }

            Cv? cv = await _db.Cvs
                .Include(c => c.Skills)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .FirstOrDefaultAsync(c => c.UserId == cvUser.Id);

            if (cv == null)
            {
                return NotFound();
            }

            CvDetailsViewModel cvVm = new CvDetailsViewModel
            {
                User = cvUser,
                Cv = cv

            };

            if (isLoggedIn && viewerId != cvUser.Id)
            {
                var existingViewer = await _db.CvVisits
                    .AnyAsync(v => v.CvId == cv.Id && v.VisitorId == viewerId);

                if (!existingViewer)
                {
                    _db.CvVisits.Add(new CvVisit
                    {
                        CvId = cv.Id,
                        VisitorId = viewerId
                    });
                    await _db.SaveChangesAsync();
                }

            }


            //Jag lämnar detta här som exempel på hur man tar fram antalet visningar.
            /*var viewercount = await _db.CvVisits
                .Where(v => v.CvId == cv.Id)
                .CountAsync();*/

            return View("Details", cvVm);
        }
    }
}

