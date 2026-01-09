using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.ViewModels
{

    public class LoginViewModel
    {
        [Required(ErrorMessage = "*")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "*")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}