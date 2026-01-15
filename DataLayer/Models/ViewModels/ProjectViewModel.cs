namespace DataLayer.Models.ViewModels
{
    public class ProjectViewModel
    {
        public Project Project { get; set; } = new Project();
        public IEnumerable<User>? Participants { get; set; }
        public string OwnerImageUrl { get; set; } = "/images/default-profile2.png";
        public string? OwnerText { get; set; }
        public bool IsOwnerHidden =>
            OwnerText == "Inaktiverat konto" || OwnerText == "Privat konto";
    }

    
}
