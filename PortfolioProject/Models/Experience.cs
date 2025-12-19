namespace PortfolioProject.Models
{
    public class Experience
    {
        public Guid ExperienceId {get;set;} = Guid.NewGuid();
        public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();

        public string Company { get; set; }
        public string Role { get; set; }

        public DateTime StartYear { get; set; }
        public DateTime EndYear { get; set; }
    }
}