using DataLayer.Models.InputModels;

namespace DataLayer.Models.ViewModels
{
    public class StartAnonymousThreadViewModel
    {
        public string RecipientUsername { get; set; } = "";
        public string RecipientFullName { get; set; } = "";
        public string? RecipientProfileImageUrl { get; set; }

        // Form input
        public StartAnonymousInputModel Input { get; set; } = new();
    }
}
