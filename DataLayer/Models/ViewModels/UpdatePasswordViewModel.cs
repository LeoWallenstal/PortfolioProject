using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.ViewModels
{
    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = "Får inte vara tom!")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Lösenorden måste vara minst 6 tecken lång!")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte!")]
        [Required(ErrorMessage = "")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Lösenorden måste vara minst 6 tecken lång!")]
        public string ConfirmNewPassword { get; set; }

        [Required(ErrorMessage = "Får inte vara tom!")]
        [DataType(DataType.Password)]
        
        public string ConfirmPassword { get; set; }

    }
}
