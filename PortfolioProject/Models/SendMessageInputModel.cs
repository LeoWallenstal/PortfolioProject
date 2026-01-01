using System.ComponentModel.DataAnnotations;

namespace PortfolioProject.Models
{
    public class SendMessageInputModel
    {
        [Required, StringLength(3500)]
        public string Body { get; set; }
    }
}
