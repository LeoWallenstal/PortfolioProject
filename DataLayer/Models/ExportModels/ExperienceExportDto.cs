using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.ExportModels
{
    public class ExperienceExportDto
    {
        public string Company { get; set; }
        public string Role { get; set; }
        public DateTime StartYear { get; set; }
        public DateTime EndYear { get; set; }
    }
}
