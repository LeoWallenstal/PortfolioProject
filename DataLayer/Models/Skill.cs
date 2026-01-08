namespace DataLayer.Models
{
    public class Skill
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }
        public string? ImageUrl { get; set; }

        public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();
    }
}