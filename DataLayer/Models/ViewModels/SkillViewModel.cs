using DataLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.ViewModels
{
    public class SkillViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Url]
        public string? ImageUrl { get; set; }
    }
}