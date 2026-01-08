using DataLayer.Models;

namespace DataLayer.Models.ViewModels
{
    public class SkillViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
    }
}