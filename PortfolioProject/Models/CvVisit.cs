namespace PortfolioProject.Models
{
    public class CvVisit
    {
        public Guid Id { get; set; }
        public Guid CvId { get; set; }
        public virtual Cv Cv { get; set; } = default!;

        public string? VisitorId { get; set; }
        public virtual User? Visitor {  get; set; }
    }
}
