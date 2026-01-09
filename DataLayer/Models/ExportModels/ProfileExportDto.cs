using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataLayer.Models.ExportModels
{
    [XmlRoot("ProfileExport")]
    public class ProfileExportDto
    {
        public DateTime ExportDate = DateTime.UtcNow;
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Adress {  get; set; }

        public CvExportDto? Cv { get; set; }

        [XmlArray("Projects")]
        [XmlArrayItem("Project")]
        public List<ProjectExportDto>? Projects { get; set; }
    }
}
