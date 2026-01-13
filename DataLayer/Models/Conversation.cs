namespace DataLayer.Models
{
    public class Conversation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? UserAId { get; set; }
        public virtual User? UserA { get; set; }
        
        //UserB är null när man har en konversation med en anoynym gäst.
        public string? UserBId { get; set; } 
        public virtual User? UserB { get; set; }

        public bool IsAnonymous { get; set; } = false;
        public Guid? PublicId { get; set; } 
        public string? AnonymousDisplayName { get; set; } 
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
