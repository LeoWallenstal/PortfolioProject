using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.ViewModels
{
    public class CvProfileViewModel
    {
        // --- User / profile ---
        public string UserName { get; set; } = "";
        public string? FullName { get; set; }
        public string? ProfileImagePath { get; set; }
        public bool IsPrivate { get; set; }

        // --- CV details ---
        public int ViewCount { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public ICollection<SkillViewModel> Skills { get; set; } = new List<SkillViewModel>();
        public ICollection<EducationViewModel> Educations { get; set; } = new List<EducationViewModel>();
        public ICollection<ExperienceViewModel> Experiences { get; set; } = new List<ExperienceViewModel>();

        public CvProfileViewModel(User user)
        {
            //User
            UserName = user.UserName;
            FullName = $"{user.FirstName} {user.LastName}";
            ProfileImagePath = user.ProfileImageUrl;
            IsPrivate = user.IsPrivate;


            if (user.Cv != null)
            {
                //CV
                ViewCount = user.Cv.ViewCount;
                Title = user.Cv.Title ?? "";
                Summary = user.Cv.Summary;

                Skills = user.Cv.Skills
                    .Select(s => new SkillViewModel
                    {
                        Id = s.Id,
                        Name = s.Name,
                        ImageUrl = s.ImageUrl
                    })
                    .ToList() ?? new List<SkillViewModel>();

                Educations = user.Cv.Educations
                    .Select(e => new EducationViewModel
                    {
                        EducationId = e.EducationId,
                        School = e.School,
                        Degree = e.Degree,
                        StartYear = e.StartYear,
                        EndYear = e.EndYear,
                    })
                    .ToList() ?? new List<EducationViewModel>();

                Experiences = user.Cv.Experiences
                    .Select(e => new ExperienceViewModel
                    {
                        ExperienceId = e.ExperienceId,
                        Company = e.Company,
                        Role = e.Role,
                        StartYear = e.StartYear,
                        EndYear = e.EndYear,
                    })
                    .ToList() ?? new List<ExperienceViewModel>();
            }
        }

        public CvProfileViewModel() { }
    }
}
