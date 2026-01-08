using System.ComponentModel.DataAnnotations;

namespace DataLayer.Models.InputModels
{
    public class SendMessageInputModel
    {
        [Required, StringLength(3500)]
        public string Body { get; set; }
    }
}
