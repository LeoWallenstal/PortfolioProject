namespace DataLayer.Models.ViewModels
{
    public class ProjectViewModel
    {
        public Project Project { get; set; } = new Project();
        public IEnumerable<User>? Participants { get; set; }
        public string OwnerImageUrl { get; set; } = null!;
        public string OwnerText { get; set; } = null!;
        public bool IsOwnerHidden =>
            OwnerText == "Inaktiverat konto" || OwnerText == "Privat konto";
    }
}
