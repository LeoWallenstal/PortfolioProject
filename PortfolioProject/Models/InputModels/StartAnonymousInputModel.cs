using System.ComponentModel.DataAnnotations;

namespace PortfolioProject.Models.InputModels
{
    public class StartAnonymousInputModel
    {
        [Required(ErrorMessage = "Namn krävs")]
        [StringLength(50, ErrorMessage = "Namnet får inte vara längre än 50 tecken")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Meddelande krävs")]
        [StringLength(3500, ErrorMessage = "Meddelandet får inte vara längre än 3500 tecken")]
        public string Message { get; set; } = string.Empty;

        [StringLength(50,ErrorMessage = "Lösenordet får inte vara längre än 50 tecken")]
        public string? Password { get; set; }
    }
}
