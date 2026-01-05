namespace PortfolioProject.Models
{
    public class Conversation
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? UserAId { get; set; }
        public string? UserBId { get; set; }

        public virtual User? UserA { get; set; }
        public virtual User? UserB { get; set; }

        //public bool IsAnonymous { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
