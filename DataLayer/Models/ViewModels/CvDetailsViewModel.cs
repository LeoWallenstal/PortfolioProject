namespace DataLayer.Models.ViewModels
{
    public class CvDetailsViewModel
    {
        // --- User / profile ---
        public string UserName { get; set; } = "";
        public string? FullName { get; set; }
        public string? ProfileImagePath { get; set; }
        public bool IsPrivate { get; set; }

        // --- CV details ---
        public int? ViewCount { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    }
}
