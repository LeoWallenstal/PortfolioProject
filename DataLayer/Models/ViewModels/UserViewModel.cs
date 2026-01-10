using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.ViewModels
{
    public class UserViewModel
    {

        public string UserName { get; set; }
        public ProfileViewModel Profile { get; set; }
        public UpdatePasswordViewModel pwVm { get; set; } = new UpdatePasswordViewModel();
        public CvProfileViewModel? Cv { get; set; } = new CvProfileViewModel();
        public List<ProjectViewModel> Projects { get; set; } = new List<ProjectViewModel>();
        public UserViewModel(User aUser) {
            Cv = new CvProfileViewModel(aUser);
            Profile = new ProfileViewModel(aUser);
            UserName = aUser.UserName;
        }

        public string FullName => Profile.FullName;

        //For modelbinding
        public UserViewModel() { }

        public bool CvIsEmpty() {
            return !Cv.Skills.Any() && !Cv.Experiences.Any()
                && !Cv.Experiences.Any() && Cv.Title.IsNullOrEmpty()
                && Cv.Summary.IsNullOrEmpty() && Cv.GitHubUrl.IsNullOrEmpty()
                && Cv.LinkedInUrl.IsNullOrEmpty() && Cv.XUrl.IsNullOrEmpty();
                
        }
    }
}
