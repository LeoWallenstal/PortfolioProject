namespace DataLayer.Models.ViewModels
{
    public class ConversationListItemViewModel
    {
        public Guid ConversationId { get; set; }
        public string? OtherUsersId { get; set; }
        public string? OtherUsersFullName { get; set; }
        public string? OtherUsername { get; set; }
        public string? OtherUsersProfileImageUrl { get; set; } = "/images/default-profile2.png";
        public string? Preview { get; set; }
        public DateTime LastSentAt { get; set; }
        public bool LastMessageIsMine { get; set; }
        public int UnreadCount { get; set; } 
    }
}
