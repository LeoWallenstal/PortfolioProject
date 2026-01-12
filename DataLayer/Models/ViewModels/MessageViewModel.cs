namespace DataLayer.Models.ViewModels
{
    public class MessageViewModel
    {
        public Guid MessageId { get; set; }
        public bool IsMine { get; set; }
        public DateTime SentAt { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsDeletedByReceiver { get; set; } = false;
    }
}
