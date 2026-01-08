using DataLayer.Models;

namespace DataLayer.Models.ViewModels
{
    public class EducationViewModel
    {
        public Guid EducationId { get; set; }
        public string School { get; set; }
        public string Degree { get; set; }
        public DateTime StartYear { get; set; }
        public DateTime EndYear { get; set; }
    }
}