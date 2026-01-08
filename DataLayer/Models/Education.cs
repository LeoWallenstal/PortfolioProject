namespace DataLayer.Models
{
    public class Education
    {
        public Guid EducationId {get;set;} = Guid.NewGuid();
        public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();
        public string School { get; set; }
        public string Degree { get; set; }
        public DateTime StartYear { get; set; }
        public DateTime EndYear { get; set; }
    }
}