using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioProject.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Adress { get; set; }
        public bool IsPrivate { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string ProfileImageUrl { get; set; } = "/images/default-profile2.png";

        public virtual Cv? Cv { get; set; } 
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    }
}
