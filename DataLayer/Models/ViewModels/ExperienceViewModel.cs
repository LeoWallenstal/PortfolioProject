using DataLayer.Models;

namespace DataLayer.Models.ViewModels
{
    public class ExperienceViewModel
    {
        public Guid ExperienceId { get; set; }

        public string Company { get; set; }
        public string Role { get; set; }

        public DateTime StartYear { get; set; }
        public DateTime EndYear { get; set; }
    }
}