using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models.ExportModels
{
    public class EducationExportDto
    {
        public string School { get; set; }
        public string Degree { get; set; }
        public DateTime StartYear { get; set; }
        public DateTime EndYear { get; set; }
    }
}
