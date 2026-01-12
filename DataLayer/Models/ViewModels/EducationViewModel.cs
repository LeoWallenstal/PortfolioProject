using DataLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.ViewModels
{
    public class EducationViewModel
    {
        public Guid EducationId { get; set; }
        [Required]
        public string? School { get; set; }
        [Required]
        public string? Degree { get; set; }
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