namespace PortfolioProject.Models
{
    public class ProjectViewModel
    {
        public Project Project { get; set; }
        public IEnumerable<User> VisibleUsers { get; set; }
    }
}
