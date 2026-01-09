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
        private readonly ILogger<CvController> _logger;

        public CvController(DatabaseContext db, UserManager<User> userManager, ILogger<CvController> logger)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Details(string username)
        {
            var cvUser = await _db.Users.Include(u => u.Projects)
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (cvUser == null)
                return NotFound();

            var isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            if (cvUser.IsPrivate && !isLoggedIn)
                return View("Private");

            Cv? cv = await _db.Cvs
                .Include(c => c.Skills)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .FirstOrDefaultAsync(c => c.UserId == cvUser.Id);

            if (cv == null)
                return NotFound();

            

            var viewerId = _userManager.GetUserId(User);

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

            /*var viewercount = await _db.CvVisits
                .Where(v => v.CvId == cv.Id)
                .CountAsync();*/

            //await TestFindSimilar(cv);

            var similarCvIds = await TestFindSimilar2(cv);
  
            var similarCvList = await _db.Cvs.Where(cv => similarCvIds.Contains(cv.Id)).ToListAsync();

            CvDetailsViewModel cvVm = new CvDetailsViewModel
            {
                User = cvUser,
                Cv = cv,
                SimilarCvs = similarCvList,
            };

            return View("Details", cvVm);
        }
        private async Task TestFindSimilar(Cv usersCv)
        {
            var userSkills = usersCv.Skills.Select(s => s.Name.ToLower()).ToHashSet();
            var userSchools = usersCv.Educations.Select(e => e.School.ToLower()).ToHashSet();
            var userDegrees = usersCv.Educations.Select(e => e.Degree.ToLower()).ToHashSet();
            var userCompanies = usersCv.Experiences.Select(e => e.Company.ToLower()).ToHashSet();
            var userRoles = usersCv.Experiences.Select(e => e.Role.ToLower()).ToHashSet();


            _logger.LogInformation($"Calculating similarityscore for {usersCv.User.FirstName}");
            var candidates = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id != usersCv.UserId && u.Cv != null && !u.IsPrivate && u.IsActive)
                .Include(u => u.Cv).ThenInclude(cv => cv.Skills)
                .Include(u => u.Cv).ThenInclude(cv => cv.Educations)
                .Include(u => u.Cv).ThenInclude(cv => cv.Experiences)
                .ToListAsync();


            foreach (var candidate in candidates)
            {

                var candidateSkills = candidate.Cv.Skills.Select(s => s.Name.ToLower()).ToHashSet();
                double skillScore = CalculateJaccard(userSkills, candidateSkills);


                var candidateSchools = candidate.Cv.Educations.Select(e => e.School.ToLower()).ToHashSet();
                double schoolScore = CalculateJaccard(userSchools, candidateSchools);


                var candidateDegrees = candidate.Cv.Educations.Select(e => e.Degree.ToLower()).ToHashSet();
                double degreeScore = CalculateJaccard(userDegrees, candidateDegrees);


                var candidateCompanies = candidate.Cv.Experiences.Select(e => e.Company.ToLower()).ToHashSet();
                double companyScore = CalculateJaccard(userCompanies, candidateCompanies);


                var candidateRoles = candidate.Cv.Experiences.Select(e => e.Role.ToLower()).ToHashSet();
                double roleScore = CalculateJaccard(userRoles, candidateRoles);

                //Decimalerna bestämmer hur många procent av den totala likheten som dem enskilda attributen står för.
                var totalScore =
                    0.4 * skillScore +
                    0.1 * schoolScore +
                    0.1 * degreeScore +
                    0.2 * companyScore +
                    0.2 * roleScore;

                _logger.LogInformation($"User {candidate.UserName} had a total score of {totalScore}");
            }
        }

        private async Task<List<Guid>> TestFindSimilar2(Cv usersCv)
        {
            var cvIds = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id != usersCv.UserId && u.Cv != null && !u.IsPrivate && u.IsActive)
                .Select(u => u.Cv.Id)
                .ToListAsync();

            var skillsByCv = await _db.Cvs
                .AsNoTracking()
                .Where(cv => cvIds.Contains(cv.Id))
                .Select(cv => new
                {
                    cv.Id,
                    Skills = cv.Skills
                        .Select(s => s.Name == null ? "" : s.Name.Trim().ToLower())
                        .Where(x => x != "")
                })
                 .ToDictionaryAsync(x => x.Id, x => x.Skills.ToHashSet());

            var educByCv = await _db.Cvs.AsNoTracking()
                .Where(cv => cvIds.Contains(cv.Id))
                .Select(cv => new
                {
                    cv.Id,
                    Schools = cv.Educations
                        .Select(e => e.School == null ? "" : e.School.Trim().ToLower())
                        .Where(x => x != ""),
                    Degrees = cv.Educations
                        .Select(e => e.Degree == null ? "" : e.Degree.Trim().ToLower())
                        .Where(x => x != "")
                })
                .ToDictionaryAsync(x => x.Id, x => new
                {
                    Schools = x.Schools.ToHashSet(),
                    Degrees = x.Degrees.ToHashSet()
                });

            var expByCv = await _db.Cvs.AsNoTracking()
                .Where(cv => cvIds.Contains(cv.Id))
                .Select(cv => new
                {
                    cv.Id,
                    Companies = cv.Experiences
                        .Select(e => e.Company == null ? "" : e.Company.Trim().ToLower())
                        .Where(x => x != ""),
                    Roles = cv.Experiences
                        .Select(e => e.Role == null ? "" : e.Role.Trim().ToLower())
                        .Where(x => x != "")
                })
                .ToDictionaryAsync(x => x.Id, x => new
                {
                    Companies = x.Companies.ToHashSet(),
                    Roles = x.Roles.ToHashSet()
                });

            _logger.LogInformation($"Calculating similarityscore for {usersCv.User.FirstName}");

            string Norm(string? s) => (s ?? "").Trim().ToLowerInvariant();

            var userSkills = usersCv.Skills.Select(s => Norm(s.Name)).Where(x => x != "").ToHashSet();
            var userSchools = usersCv.Educations.Select(e => Norm(e.School)).Where(x => x != "").ToHashSet();
            var userDegrees = usersCv.Educations.Select(e => Norm(e.Degree)).Where(x => x != "").ToHashSet();
            var userCompanies = usersCv.Experiences.Select(e => Norm(e.Company)).Where(x => x != "").ToHashSet();
            var userRoles = usersCv.Experiences.Select(e => Norm(e.Role)).Where(x => x != "").ToHashSet();

            var results = new Dictionary<Guid, double>();

            foreach (var id in cvIds)
            {
                double skillScore = CalculateJaccard(userSkills, skillsByCv[id]);
                double schoolScore = CalculateJaccard(userSchools, educByCv[id].Schools);
                double degreeScore = CalculateJaccard(userDegrees, educByCv[id].Degrees);
                double companyScore = CalculateJaccard(userCompanies, expByCv[id].Companies);
                double roleScore = CalculateJaccard(userRoles, expByCv[id].Roles);

                //Decimalerna bestämmer hur många procent av den totala likheten som dem enskilda attributen står för.
                var totalScore =
                    0.4 * skillScore +
                    0.1 * schoolScore +
                    0.1 * degreeScore +
                    0.2 * companyScore +
                    0.2 * roleScore;

                if (totalScore <= 0.1)
                    continue;

                results[id] = totalScore;

                _logger.LogInformation($"cvId {id} had a total score of {totalScore}");
            }

            var topMatches = results
                .OrderByDescending(result => result.Value)
                .Select(result => result.Key)
                .ToList();

            return topMatches;
        }

        private static double CalculateJaccard(HashSet<string> aAttributes, HashSet<string> bAttributes)
        {
            if (!aAttributes.Any() || !bAttributes.Any())
                return 0;

            var sharedAttributes = aAttributes.Intersect(bAttributes).Count();
            var totalAttributes = aAttributes.Union(bAttributes).Count();

            return (double)sharedAttributes / totalAttributes;
        }


    }


}

