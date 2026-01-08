using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Kan inte vara tom!")]
        [RegularExpression(@"^[A-Za-z-]+$", ErrorMessage = "Endast A-Z och -")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Kan inte vara tom!")]
        [RegularExpression(@"^[A-Za-z-]+$", ErrorMessage = "Endast A-Z och -")]
        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string UserName { get; set; }

        [Required(ErrorMessage = "Kan inte vara tom!")]
        [EmailAddress(ErrorMessage = "Ogilitigt format!")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Ogiltigt format!")]
        public string PhoneNumber { get; set; }

        [RegularExpression(@"^[0-9+\-()\s]+$", ErrorMessage = "Ogiltigt format!")]
        public string? Adress { get; set; }

        public bool IsPrivate { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string ProfileImageUrl { get; set; } = "/images/default-profile2.png";

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }

        public CvProfileViewModel? Cv { get; set; } = new CvProfileViewModel();

        public List<ProjectViewModel> Projects { get; set; } = new List<ProjectViewModel>();

        public UserViewModel(User aUser) { 
            FirstName = aUser.FirstName;
            LastName = aUser.LastName;
            UserName = aUser.UserName;
            Email = aUser.Email;
            PhoneNumber = aUser.PhoneNumber;
            Adress = aUser.Adress;
            IsPrivate = aUser.IsPrivate;
            IsActive = aUser.IsActive;
            ProfileImageUrl = aUser.ProfileImageUrl;

            Cv = new CvProfileViewModel(aUser);
        }

        //For modelbinding
        public UserViewModel() { }

        public bool CvIsEmpty() { 
            return !Cv.Skills.Any() && !Cv.Experiences.Any() && !Cv.Experiences.Any();
        }
    }
}
