namespace DataLayer.Models.ViewModels
{
    public class CvDetailsViewModel
    {
        public Cv Cv { get; set; }
        public List<Cv> SimilarCvs { get; set; }
        public User User { get; set; }


    }
}
