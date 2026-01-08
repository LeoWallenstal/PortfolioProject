using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class Cv{
        
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }
        public virtual User User { get; set; }
        public int ViewCount { get; set; } = 0;
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? XUrl { get; set; }

        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

        public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

        public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();

    }
}
