using System.ComponentModel.DataAnnotations;

namespace PortfolioProject.Models
{
    public class UserViewModel
    {
        [Required]
        [RegularExpression(@"^[A-Za-z-]+$", ErrorMessage = "Only letters and hyphens allowed!")]
        public string? FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z-]+$", ErrorMessage = "Only letters and hyphens allowed!")]
        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
        public string? Adress { get; set; }
        public bool IsPrivate { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string ProfileImageUrl { get; set; } = "/images/default-profile2.png";

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }

        public Cv? Cv { get; set; }
        public List<ProjectViewModel> Projects { get; set; } = new List<ProjectViewModel>();

        public List<MessageViewModel> SentMessages { get; set; } = new List<MessageViewModel>();
        public List<MessageViewModel> ReceivedMessages { get; set; } = new List<MessageViewModel>();

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
            //Kanske CV, Projects, SentMessages, RecievedMessages
        }

        //For modelbinding
        public UserViewModel() { }
    }
}
