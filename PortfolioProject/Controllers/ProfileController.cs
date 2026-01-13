using DataLayer;
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
        private readonly IWebHostEnvironment _env; 

        public ProfileController(DatabaseContext dbContext, UserManager<User> userManager, IWebHostEnvironment env) {
            _userManager = userManager;
            _dbContext = dbContext;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            user.Cv.ViewCount = await _dbContext.CvVisits.Where(v => v.CvId == user.Cv.Id).CountAsync();

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
        public async Task<IActionResult> EditProfile(UserViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                // Debug output
                foreach (var kvp in ModelState)
                {
                    var key = kvp.Key;
                    var errors = kvp.Value.Errors;

                    foreach (var error in errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        Debug.WriteLine($"{key}, {errors}, {errorMessage}, {exception}");
                    }
                }
                return View(vm);
            }

            user.FirstName = vm.Profile.FirstName;
            user.LastName = vm.Profile.LastName;
            user.Adress = vm.Profile.Adress;
            user.Email = vm.Profile.Email;
            user.PhoneNumber = vm.Profile.PhoneNumber;
            user.IsPrivate = vm.Profile.IsPrivate;
            user.IsActive = vm.Profile.IsActive;

            // Profile pic upload
            if (vm.Profile.ProfileImageUpload != null && vm.Profile.ProfileImageUpload.Length > 0)
            {
                // Delete old file if not default
                if (!string.IsNullOrEmpty(user.ProfileImageUrl) && !user.ProfileImageUrl.Contains("default-profile2.png"))
                {
                    var oldFile = Path.Combine(_env.WebRootPath, user.ProfileImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFile))
                        System.IO.File.Delete(oldFile);
                }
                    
                // Save new file
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(vm.Profile.ProfileImageUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await vm.Profile.ProfileImageUpload.CopyToAsync(stream);
                }

                // Update user profile image path
                user.ProfileImageUrl = "/uploads/" + fileName;
            }

            // Update user in database
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(vm);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePassword() {
            
            return View(new UpdatePasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid) {
                return View(vm);
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                vm.ConfirmPassword, 
                vm.ConfirmNewPassword
            );

            if (!result.Succeeded) { 
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        nameof(RegisterViewModel.Password),
                        error.Description
                    );
                }
            }

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
                Debug.WriteLine("\n\n\n\n[CreateCv][POST][DEBUG]:");
                // FÖR DEBUG
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine(error.ErrorMessage);
                }
                Debug.WriteLine("\n\n\n\n");
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
                    await _dbContext.SaveChangesAsync();
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
                }
            }

            // ------------------- SAVE CV -------------------
            _dbContext.Cvs.Add(toCreate);
            await _dbContext.SaveChangesAsync();

            //Redirect back to Index(?)
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditCv() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            UserViewModel uVm = new UserViewModel(user);

            return View(uVm.Cv);
        }

        [HttpPost]
        public async Task<IActionResult> EditCv(CvProfileViewModel cvVm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Debug.WriteLine($"[Error] {kvp.Key}: {error.ErrorMessage}");
                    }
                }
                return View(cvVm);
            }

            var cv = await _dbContext.Cvs
                .Include(c => c.Skills)
                .Include(c => c.Educations)
                .Include(c => c.Experiences)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cv == null)
            {
                cv = new Cv
                {
                    UserId = user.Id
                };
                _dbContext.Cvs.Add(cv);
            }

            cv.Title = cvVm.Title;
            cv.Summary = cvVm.Summary;
            cv.GitHubUrl = cvVm.GitHubUrl;
            cv.LinkedInUrl = cvVm.LinkedInUrl;
            cv.XUrl = cvVm.XUrl;

            bool EqualsIgnoreCase(string a, string b) =>
                string.Equals(a?.Trim(), b?.Trim(), StringComparison.OrdinalIgnoreCase);

            var skillNamesFromVm = cvVm.Skills.Select(s => s.Name.Trim().ToLower()).ToHashSet();

            // Remove skills that are not in VM
            var skillsToRemove = cv.Skills
                .Where(s => !skillNamesFromVm.Contains(s.Name.Trim().ToLower()))
                .ToList();

            foreach (var s in skillsToRemove)
                cv.Skills.Remove(s);

            // Add new skills
            foreach (var sVm in cvVm.Skills)
            {
                if (cv.Skills.Any(s => EqualsIgnoreCase(s.Name, sVm.Name)))
                    continue;

                var existingSkill = await _dbContext.Skills
                    .FirstOrDefaultAsync(s => s.Name.ToLower() == sVm.Name.Trim().ToLower());

                if (existingSkill != null)
                    cv.Skills.Add(existingSkill);
                else
                {
                    var newSkill = new Skill
                    {
                        Name = sVm.Name.Trim(),
                        ImageUrl = sVm.ImageUrl?.Trim() ?? ""
                    };
                    _dbContext.Skills.Add(newSkill);
                    cv.Skills.Add(newSkill);
                }
            }

            var eduKeysFromVm = cvVm.Educations
                .Select(e => $"{e.School.Trim().ToLower()}|{e.Degree.Trim().ToLower()}|{e.StartYear}|{e.EndYear}")
                .ToHashSet();

            var eduKeysInCv = cv.Educations
                .Select(e => $"{e.School.Trim().ToLower()}|{e.Degree.Trim().ToLower()}|{e.StartYear}|{e.EndYear}")
                .ToList();

            // Remove educations not in VM
            var educationsToRemove = cv.Educations
                .Where(e => !eduKeysFromVm.Contains($"{e.School.Trim().ToLower()}|{e.Degree.Trim().ToLower()}|{e.StartYear}|{e.EndYear}"))
                .ToList();

            foreach (var e in educationsToRemove)
                cv.Educations.Remove(e);

            // Add new educations
            foreach (var eVm in cvVm.Educations)
            {
                if (cv.Educations.Any(e =>
                    EqualsIgnoreCase(e.School, eVm.School) &&
                    EqualsIgnoreCase(e.Degree, eVm.Degree) &&
                    e.StartYear == eVm.StartYear &&
                    e.EndYear == eVm.EndYear))
                    continue;

                var existingEdu = await _dbContext.Educations.FirstOrDefaultAsync(e =>
                    e.School.ToLower() == eVm.School.Trim().ToLower() &&
                    e.Degree.ToLower() == eVm.Degree.Trim().ToLower() &&
                    e.StartYear == eVm.StartYear &&
                    e.EndYear == eVm.EndYear);

                if (existingEdu != null)
                    cv.Educations.Add(existingEdu);
                else
                {
                    var newEdu = new Education
                    {
                        School = eVm.School.Trim(),
                        Degree = eVm.Degree.Trim(),
                        StartYear = eVm.StartYear,
                        EndYear = eVm.EndYear
                    };
                    _dbContext.Educations.Add(newEdu);
                    cv.Educations.Add(newEdu);
                }
            }

            var expKeysFromVm = cvVm.Experiences
                .Select(e => $"{e.Company.Trim().ToLower()}|{e.Role.Trim().ToLower()}|{e.StartYear}|{e.EndYear}")
                .ToHashSet();

            var experiencesToRemove = cv.Experiences
                .Where(e => !expKeysFromVm.Contains($"{e.Company.Trim().ToLower()}|{e.Role.Trim().ToLower()}|{e.StartYear}|{e.EndYear}"))
                .ToList();

            foreach (var e in experiencesToRemove)
                cv.Experiences.Remove(e);

            foreach (var eVm in cvVm.Experiences)
            {
                if (cv.Experiences.Any(e =>
                    EqualsIgnoreCase(e.Company, eVm.Company) &&
                    EqualsIgnoreCase(e.Role, eVm.Role) &&
                    e.StartYear == eVm.StartYear &&
                    e.EndYear == eVm.EndYear))
                    continue;

                var existingExp = await _dbContext.Experiences.FirstOrDefaultAsync(e =>
                    e.Company.ToLower() == eVm.Company.Trim().ToLower() &&
                    e.Role.ToLower() == eVm.Role.Trim().ToLower() &&
                    e.StartYear == eVm.StartYear &&
                    e.EndYear == eVm.EndYear);

                if (existingExp != null)
                    cv.Experiences.Add(existingExp);
                else
                {
                    var newExp = new Experience
                    {
                        Company = eVm.Company.Trim(),
                        Role = eVm.Role.Trim(),
                        StartYear = eVm.StartYear,
                        EndYear = eVm.EndYear
                    };
                    _dbContext.Experiences.Add(newExp);
                    cv.Experiences.Add(newExp);
                }
            }

            // --- Save all changes ---
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost] 
        public async Task<IActionResult> ExportProfile() {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            ExportPlaceholder exporter = new ExportPlaceholder(_dbContext, _userManager);

            ExportFileResult? toReturn =  await exporter.ExportProfileXmlAsync(user.Id);
            return File(toReturn.Bytes, toReturn.ContentType, toReturn.FileName);
        }

    }
}
