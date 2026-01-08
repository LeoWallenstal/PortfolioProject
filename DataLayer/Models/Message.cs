namespace DataLayer.Models
{
    public class Message{
        public Guid MessageId {get;set;} = Guid.NewGuid();

        public Guid ConversationId { get; set; }
        public virtual Conversation Conversation { get; set; } = null!;

        public string? FromUserId {get; set; }
        public virtual User? FromUser {get; set;} 

        public string? ToUserId { get; set; }
        public virtual User? ToUser { get; set; }

        public string? AnonymousDisplayName {get; set; }

        public string Body { get; set; }
        
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}