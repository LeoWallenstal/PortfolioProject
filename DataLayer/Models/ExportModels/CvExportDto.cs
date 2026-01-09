using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataLayer.Models.ExportModels
{
    public class CvExportDto
    {
        public int ViewCount { get; set; }
        public string? Title { get; set; }
        public string? Summary { get; set; }
        public string? GitHubUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? XUrl { get; set; }

        [XmlArray("Skills")]
        [XmlArrayItem("Skill")]
        public List<SkillExportDto> Skills { get; set; } = new();

        [XmlArray("Educations")]
        [XmlArrayItem("Education")]
        public List<EducationExportDto> Educations { get; set; } = new();

        [XmlArray("Experiences")]
        [XmlArrayItem("Experience")]
        public List<ExperienceExportDto> Experiences { get; set; } = new();
    }
}
