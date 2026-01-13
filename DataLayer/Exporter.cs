using DataLayer.Data;
using DataLayer.Models;
using DataLayer.Models.ExportModels;
using DataLayer.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DataLayer
{
    public class Exporter
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<User> _userManager;
        public Exporter(DatabaseContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ExportFileResult?> ExportProfileXmlAsync(string userId)
        {
            var user = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Cv)
                .ThenInclude(cv => cv.Skills)
            .Include(u => u.Cv)
                .ThenInclude(cv => cv.Educations)
            .Include(u => u.Cv)
                .ThenInclude(cv => cv.Experiences)
            .Include(u => u.Projects)
            .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) { return null; }

            var dto = new ProfileExportDto
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Adress = user.Adress,
                Cv = user.Cv is null ? new CvExportDto { } : new CvExportDto
                {
                    ViewCount = user.Cv.ViewCount,
                    Title = user.Cv.Title,
                    Summary = user.Cv.Summary,
                    GitHubUrl = user.Cv.GitHubUrl,
                    LinkedInUrl = user.Cv.LinkedInUrl,
                    XUrl = user.Cv.XUrl,
                    Skills = user.Cv.Skills.Select(s => new SkillExportDto
                    {
                        Name = s.Name
                    }).ToList(),
                    Educations = user.Cv.Educations.Select(e => new EducationExportDto
                    {
                        School = e.School,
                        Degree = e.Degree,
                        StartYear = e.StartYear,
                        EndYear = e.EndYear
                    }).ToList(),
                    Experiences = user.Cv.Experiences.Select(e => new ExperienceExportDto
                    {
                        Company = e.Company,
                        Role = e.Role,
                        StartYear = e.StartYear,
                        EndYear = e.EndYear
                    }).ToList(),
                },
                Projects = user.Projects.Select(p => new ProjectExportDto
                {
                    Title = p.Title,
                    Description = p.Description,
                    CreatedDate = p.CreatedDate
                }).ToList()
            };
        
            var serializer = new XmlSerializer(typeof(ProfileExportDto));

            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
                OmitXmlDeclaration = false
            };

            byte[] bytes;
            using (var ms = new MemoryStream())
            using (var writer = XmlWriter.Create(ms, settings))
            {
                serializer.Serialize(writer, dto);
                bytes = ms.ToArray();
            }

            // 4) Tvinga download i browsern
            var fileName = $"profile-export-{user.UserName}-{DateTime.UtcNow:yyyyMMdd}.xml";
            return new ExportFileResult
            {
                Bytes = bytes,
                FileName = fileName,
                ContentType = "application/xml"
            };
        }
    }

    //En custom klass för att lätt kunna skicka tillbaka resultatet till controllern.
    public sealed class ExportFileResult
    {
        public required byte[] Bytes { get; init; }
        public required string FileName { get; init; }
        public string ContentType { get; init; } = "application/xml";
    }

}
