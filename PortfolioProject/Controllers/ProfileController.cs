using DataLayer.Data;
using DataLayer.Models;
using DataLayer.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace PortfolioProject.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DatabaseContext _dbContext;

        public ProfileController(DatabaseContext dbContext, UserManager<User> userManager) {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            return View(new UserViewModel(user));
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            ModelState.Clear(); // clears any previous validation messages

            var viewModel = new UserViewModel(user); // create ViewModel from DB entity
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ProfileViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.Adress = vm.Adress;
            user.Email = vm.Email;
            user.PhoneNumber = vm.PhoneNumber;
            user.IsPrivate = vm.IsPrivate;
            user.IsActive = vm.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(vm);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(
                user,
                vm.ConfirmPassword, 
                vm.ConfirmNewPassword
            );

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CreateCv(){
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
                return NotFound();

            ModelState.Clear();

            var viewModel = new CvProfileViewModel(user);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCv(CvProfileViewModel cvVm) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // FÖR DEBUG
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
                }
                return View(cvVm);
            }

            // Create a new CV
            Cv toCreate = new Cv
            {
                UserId = user.Id,
                Title = cvVm.Title,
                Summary = cvVm.Summary,
                GitHubUrl = cvVm.GitHubUrl,
                LinkedInUrl = cvVm.LinkedInUrl,
                XUrl = cvVm.XUrl
            };

            // ------------------- SKILLS -------------------
            var skillNames = cvVm.Skills.Select(s => s.Name.Trim().ToLower()).ToList();
            var existingSkills = await _dbContext.Skills
                .Where(s => skillNames.Contains(s.Name.ToLower()))
                .ToListAsync();

            foreach (var sVm in cvVm.Skills)
            {
                var normalizedName = sVm.Name.Trim();
                var skill = existingSkills.FirstOrDefault(s => s.Name.ToLower() == normalizedName.ToLower());
                if (skill != null)
                {
                    toCreate.Skills.Add(skill);
                }
                else
                {
                    var newSkill = new Skill
                    {
                        Name = normalizedName,
                        ImageUrl = sVm.ImageUrl?.Trim() ?? ""
                    };
                    toCreate.Skills.Add(newSkill);
                    _dbContext.Skills.Add(newSkill);
                }
            }

            // ------------------- EDUCATIONS -------------------
            var schoolNames = cvVm.Educations.Select(e => e.School.Trim().ToLower()).ToList();
            var existingSchools = await _dbContext.Educations
                .Where(e => schoolNames.Contains(e.School.ToLower()))
                .ToListAsync();

            foreach (var eVm in cvVm.Educations)
            {
                var normalizedSchool = eVm.School.Trim();
                var school = existingSchools.FirstOrDefault(s => s.School.ToLower() == normalizedSchool.ToLower() &&
                                                                  s.Degree.ToLower() == eVm.Degree.Trim().ToLower() &&
                                                                  s.StartYear == eVm.StartYear &&
                                                                  s.EndYear == eVm.EndYear);
                if (school != null)
                {
                    toCreate.Educations.Add(school);
                }
                else
                {
                    var newEducation = new Education
                    {
                        School = normalizedSchool,
                        Degree = eVm.Degree.Trim(),
                        StartYear = eVm.StartYear,
                        EndYear = eVm.EndYear
                    };
                    toCreate.Educations.Add(newEducation);
                    _dbContext.Educations.Add(newEducation);
                }
            }

            // ------------------- EXPERIENCES -------------------
            var companyNames = cvVm.Experiences.Select(e => e.Company.Trim().ToLower()).ToList();
            var existingExperiences = await _dbContext.Experiences
                .Where(e => companyNames.Contains(e.Company.ToLower()))
                .ToListAsync();

            foreach (var eVm in cvVm.Experiences)
            {
                var normalizedCompany = eVm.Company.Trim();
                var experience = existingExperiences.FirstOrDefault(e =>
                    e.Company.ToLower() == normalizedCompany.ToLower() &&
                    e.Role.ToLower() == eVm.Role.Trim().ToLower() &&
                    e.StartYear == eVm.StartYear &&
                    e.EndYear == eVm.EndYear
                );

                if (experience != null)
                {
                    toCreate.Experiences.Add(experience);
                }
                else
                {
                    var newExperience = new Experience
                    {
                        Company = normalizedCompany,
                        Role = eVm.Role.Trim(),
                        StartYear = eVm.StartYear,
                        EndYear = eVm.EndYear
                    };
                    toCreate.Experiences.Add(newExperience);
                    _dbContext.Experiences.Add(newExperience);
                }
            }



            // ------------------- SAVE CV -------------------
            _dbContext.Cvs.Add(toCreate);
            await _dbContext.SaveChangesAsync();

            //Redirect back to Index(?)
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditCv(CvProfileViewModel cvVm) {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // FÖR DEBUG
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
                }
                return View(cvVm);
            }

            user.Cv.Title = cvVm.Title;
            user.Cv.Summary = cvVm.Summary;
            user.Cv.GitHubUrl = cvVm.GitHubUrl;
            user.Cv.LinkedInUrl = cvVm.LinkedInUrl;
            user.Cv.XUrl = cvVm.XUrl;


            // -------SKILLS----------
            var skillsRemove = user.Cv.Skills
                .Where(s => !cvVm.Skills.Any(vm => vm.Name.Trim().ToLower() == s.Name.ToLower()))
                .ToList();

            foreach (var s in skillsRemove)
                user.Cv.Skills.Remove(s);

            var skillNames = cvVm.Skills.Select(s => s.Name.Trim().ToLower()).ToList();
            var existingSkills = await _dbContext.Skills
                .Where(s => skillNames.Contains(s.Name.ToLower()))
                .ToListAsync();

            foreach (var sVm in cvVm.Skills)
            {
                var normalizedName = sVm.Name.Trim();
                var skill = existingSkills.FirstOrDefault(s => s.Name.ToLower() == normalizedName.ToLower());
                if (!user.Cv.Skills.Any(s => s.Name.ToLower() == normalizedName.ToLower()))
                {
                    if (skill != null)
                    {
                        user.Cv.Skills.Add(skill);
                    }
                    else
                    {
                        var newSkill = new Skill
                        {
                            Name = normalizedName,
                            ImageUrl = sVm.ImageUrl?.Trim() ?? ""
                        };
                        user.Cv.Skills.Add(newSkill);
                        _dbContext.Skills.Add(newSkill);
                    }
                }
            }

            // -------EDUCATION----------
            var educationRemove = user.Cv.Educations
                .Where(s => !cvVm.Educations.Any(vm => vm.School.Trim().ToLower() == s.School.ToLower()))
                .ToList();

            foreach (var s in educationRemove)
                user.Cv.Educations.Remove(s);

            var educationNames = cvVm.Skills.Select(s => s.Name.Trim().ToLower()).ToList();
            var existingEducations = await _dbContext.Skills
                .Where(s => skillNames.Contains(s.Name.ToLower()))
                .ToListAsync();

            foreach (var eVm in cvVm.Educations)
            {
                var normalizedName = eVm.School.Trim();
                var skill = existingEducations.FirstOrDefault(s => s.Name.ToLower() == normalizedName.ToLower());
                if (!user.Cv.Educations.Any(s => s.School.ToLower() == normalizedName.ToLower()))
                {
                    if (skill != null)
                    {
                        user.Cv.Skills.Add(skill);
                    }
                    else
                    {
                        var newEducation = new Education
                        {
                            School = normalizedName,
                            Degree = eVm.Degree,
                            StartYear = eVm.StartYear,
                            EndYear = eVm.EndYear
                        };
                        user.Cv.Educations.Add(newEducation);
                        _dbContext.Educations.Add(newEducation);
                    }
                }
            }

            // ----------EXPERIENCES----------
            var experiencesRemove = user.Cv.Experiences
                .Where(s => !cvVm.Experiences.Any(vm => vm.Company.Trim().ToLower() == s.Company.ToLower()))
                .ToList();

            foreach (var s in experiencesRemove)
                user.Cv.Experiences.Remove(s);

            var experienceNames = cvVm.Experiences.Select(s => s.Company.Trim().ToLower()).ToList();
            var existingExperiences = await _dbContext.Experiences
                .Where(s => skillNames.Contains(s.Company.ToLower()))
                .ToListAsync();

            foreach (var eVm in cvVm.Experiences)
            {
                var normalizedName = eVm.Company.Trim();
                var experience = existingExperiences.FirstOrDefault(s => s.Company.ToLower() == normalizedName.ToLower());
                if (!user.Cv.Skills.Any(s => s.Name.ToLower() == normalizedName.ToLower()))
                {
                    if (experience != null)
                    {
                        user.Cv.Experiences.Add(experience);
                    }
                    else
                    {
                        var newExperience = new Experience
                        {
                            Company = eVm.Company,
                            Role = eVm.Role,
                            StartYear = eVm.StartYear,
                            EndYear = eVm.EndYear,
                        };
                        user.Cv.Experiences.Add(newExperience);
                        _dbContext.Experiences.Add(newExperience);
                    }
                }
            }
        }
    }
}
