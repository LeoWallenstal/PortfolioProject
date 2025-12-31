namespace PortfolioProject.Models
{
    public class ConversationListItemViewModel
    {
        public string? OtherUsersId { get; set; }
        public string? OtherUsersFullName { get; set; }
        public string? OtherUsername { get; set; }
        public string? OtherUsersProfileImageUrl { get; set; }
        public string? Preview { get; set; }
        public DateTime LastSentAt { get; set; }
        public int UnreadMessageCount { get; set; } = 0;
        public bool LastMessageIsMine { get; set; }
    }
}
