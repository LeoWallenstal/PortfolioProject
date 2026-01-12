using DataLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.ViewModels
{
    public class ExperienceViewModel
    {
        public Guid ExperienceId { get; set; }

        [Required]
        public string? Company { get; set; }
        [Required]
        public string? Role { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartYear { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndYear { get; set; }
    }
}