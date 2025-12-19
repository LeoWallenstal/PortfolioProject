using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioProject.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        //public string Adress { get; set; }
        public bool isPrivate { get; set; } = false;
        public bool isActive { get; set; } = true;
        //public string ProfileImageUrl { get; set; }

        /*public virtual int CvId { get; set; }

        [ForeignKey(nameof(CvId))]
        public virtual Cv Cv { get; set; }*/

    }
}
