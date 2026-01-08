using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required(ErrorMessage = "Fyll i titel")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Fyll i beskrivning")]
        public string Description { get; set; }
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string OwnerId { get; set; }
        public virtual User? Owner { get; set; }
    }
}
