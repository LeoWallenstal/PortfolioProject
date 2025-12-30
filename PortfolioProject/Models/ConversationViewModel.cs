namespace PortfolioProject.Models
{
    public class ConversationViewModel
    {
        public string? OtherUsersId { get; set; }
        public string? OtherUsersFullName { get; set; }
        public string? OtherUsersProfileImageUrl { get; set; }
        public List<MessageViewModel> Messages { get; set; }
    }
}
