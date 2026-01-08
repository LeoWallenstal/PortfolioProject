namespace DataLayer.Models.ViewModels
{
    public class MessageViewModel
    {
        public bool IsMine { get; set; }
        public DateTime SentAt { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
