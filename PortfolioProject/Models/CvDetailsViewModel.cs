namespace PortfolioProject.Models
{
    public class CvDetailsViewModel
    {
        // --- User / profile ---
        public string UserName { get; set; } = "";
        public string? FullName { get; set; }
        public string? ProfileImagePath { get; set; }
        public bool IsPrivate { get; set; }

    }
}
