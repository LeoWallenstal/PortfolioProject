namespace PortfolioProject.Models
{
    public class ProjectViewModel
    {
        public Project Project { get; set; } = new Project();
        public User? Owner { get; set; }
        public IEnumerable<User>? Participants { get; set; }
    }
}
