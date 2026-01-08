using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.InputModels
{
    public class SendMessageInputModel
    {
        [Required(ErrorMessage = "Meddelandet får inte vara tomt.")] 
        [StringLength(3500, ErrorMessage = "Meddelandet är för långt (max 3500 tecken).")]
        public string Body { get; set; }
    }
}
