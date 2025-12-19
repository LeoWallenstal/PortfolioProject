using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioProject.Models
{
    public class Cv{
        
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }        
        [ForeignKey("UserId")]
        public virtual User user { get; set; }

        //skapa egna klasser
        public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

        public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

        public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();

        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
