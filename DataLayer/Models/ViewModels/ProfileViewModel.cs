using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DataLayer.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Kan inte vara tom!")]
        [RegularExpression(@"^[A-Za-zÅÄÖåäö]+([ -][A-Za-zÅÄÖåäö]+)*$", ErrorMessage = "Endast A-Z och -")]
        public string? FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kan inte vara tom!")]
        [RegularExpression(@"^[A-Za-zÅÄÖåäö]+([ -][A-Za-zÅÄÖåäö]+)*$", ErrorMessage = "Endast A-Z och -")]
        public string? LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}" ?? string.Empty;

        [Required(ErrorMessage = "Kan inte vara tom!")]
        [EmailAddress(ErrorMessage = "Ogiltigt format!")]
        public string? Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Ogiltigt format!")]
        public string? PhoneNumber { get; set; } = string.Empty;

        public string? Adress { get; set; } = string.Empty;

        public bool IsPrivate { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public IFormFile? ProfileImageUpload { get; set; }
        public string ProfileImagePath { get; set; } = "/images/default-profile2.png";

        // ---------- CONSTRUCTOR ----------
        public ProfileViewModel() { } // Parameterless constructor for model binding

        public ProfileViewModel(User aUser)
        {
            FirstName = aUser.FirstName ?? string.Empty;
            LastName = aUser.LastName ?? string.Empty;
            Email = aUser.Email ?? string.Empty;
            PhoneNumber = aUser.PhoneNumber ?? string.Empty;
            Adress = aUser.Adress ?? string.Empty;
            IsPrivate = aUser.IsPrivate;
            IsActive = aUser.IsActive;

            // Set the current image path for display
            ProfileImagePath = aUser.ProfileImageUrl ?? "/images/default-profile2.png";
        }
    }
}
