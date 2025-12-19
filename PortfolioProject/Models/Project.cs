using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioProject.Models
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
