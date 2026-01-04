namespace PortfolioProject.Models
{
    public class ProjectViewModel
    {
        public Project Project { get; set; } = new Project();
        public IEnumerable<User>? VisibleUsers { get; set; }
    }
}
