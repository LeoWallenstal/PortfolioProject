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

            //Kontrollerar om den inloggade användaren redan har besökt cv:t tidigare, om inte så läggs en ny visning till.
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

            //Anropar en egen metod för att hitta liknande cv:n baserat på olika attribut.
            var similarCvIds = await FindSimilarCv(cv);
  
            var similarCvList = await _db.Cvs.Where(cv => similarCvIds.Contains(cv.Id)).ToListAsync();

            CvDetailsViewModel cvVm = new CvDetailsViewModel
            {
                User = cvUser,
                Cv = cv,
                SimilarCvs = similarCvList,
            };

            return View("Details", cvVm);
        }


        private async Task<List<Guid>> FindSimilarCv(Cv usersCv)
        {
            //Hämtar alla cv-id:n för användare som har ett cv, inte är privata och är aktiva.
            var cvIds = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id != usersCv.UserId && u.Cv != null && !u.IsPrivate && u.IsActive)
                .Select(u => u.Cv.Id)
                .ToListAsync();

            //Hämtar attributen för alla dessa cv:n och sparar dem i dictionaries där nyckeln är cv-id:t och värdet är en hashset av attributen.
            //Använder trim och tolower för att normalisera strängarna för bättre jämförelse.
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

            //Normaliserar användarens attribut för att underlätta jämförelsen.
            string Norm(string? s) => (s ?? "").Trim().ToLowerInvariant();

            //Skapar hashsets för användarens attribut.
            var userSkills = usersCv.Skills.Select(s => Norm(s.Name)).Where(x => x != "").ToHashSet();
            var userSchools = usersCv.Educations.Select(e => Norm(e.School)).Where(x => x != "").ToHashSet();
            var userDegrees = usersCv.Educations.Select(e => Norm(e.Degree)).Where(x => x != "").ToHashSet();
            var userCompanies = usersCv.Experiences.Select(e => Norm(e.Company)).Where(x => x != "").ToHashSet();
            var userRoles = usersCv.Experiences.Select(e => Norm(e.Role)).Where(x => x != "").ToHashSet();

            //Dictionary för att lagra likhetsresultaten. Nyckeln är cv-id:t och värdet är likhetspoängen.
            var results = new Dictionary<Guid, double>();

            foreach (var id in cvIds)
            {
                //Beräknar Jaccard-likheten för varje attribut mellan användarens cv och det aktuella cv:t.
                double skillScore = CalculateJaccard(userSkills, skillsByCv[id]);
                double schoolScore = CalculateJaccard(userSchools, educByCv[id].Schools);
                double degreeScore = CalculateJaccard(userDegrees, educByCv[id].Degrees);
                double companyScore = CalculateJaccard(userCompanies, expByCv[id].Companies);
                double roleScore = CalculateJaccard(userRoles, expByCv[id].Roles);

                //Väger ihop poängen för att få en total likhetspoäng.
                //Decimalerna bestämmer hur många procent av den totala likheten som dem enskilda attributen står för.
                // Just nu är det 40% för skills, 10% för school, 10% för degree, 20% för company och 20% för role.
                var totalScore =
                    0.4 * skillScore +
                    0.1 * schoolScore +
                    0.1 * degreeScore +
                    0.2 * companyScore +
                    0.2 * roleScore;

                // Om totalpoängen är väldigt låg, hoppa över detta cv.
                // Detta för att undvika att visa irrelevanta resultat där exempelvis bara degree matchar.
                if (totalScore <= 0.1)
                    continue;

                results[id] = totalScore;

            }

            //Tar de 15 cv:n med högst likhetspoäng.
            var topMatches = results
                .OrderByDescending(result => result.Value)
                .Select(result => result.Key)
                .Take(15)
                .ToList();

            return topMatches;
        }

        // Beräknar Jaccard-likheten mellan två mängder av attribut.
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

