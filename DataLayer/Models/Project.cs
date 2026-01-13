using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Fyll i titel")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Titeln måste vara mellan 2 och 100 tecken lång")]

        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Fyll i beskrivning")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Beskrivningen måste vara mellan 10 och 1000 tecken lång")]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string OwnerId { get; set; }
        public virtual User? Owner { get; set; }
    }
}
