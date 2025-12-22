using System.ComponentModel.DataAnnotations;

namespace PortfolioProject.Models
{
    public class RegisterViewModel
    {
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Passwords do not match.")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
