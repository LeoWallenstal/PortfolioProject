using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Kan inte vara tom!")]
        [RegularExpression(@"^[A-Za-z-]+$", ErrorMessage = "Endast A-Z och -")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Kan inte vara tom!")]
        [RegularExpression(@"^[A-Za-z-]+$", ErrorMessage = "Endast A-Z och -")]
        public string? LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";


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

        public ProfileViewModel(User aUser) {
            FirstName = aUser.FirstName;
            LastName = aUser.LastName;
            UserName = aUser.UserName;
            Email = aUser.Email;
            PhoneNumber = aUser.PhoneNumber;
            Adress = aUser.Adress;
            IsPrivate = aUser.IsPrivate;
            IsActive = aUser.IsActive;
            ProfileImageUrl = aUser.ProfileImageUrl;
        }
    }
}
