namespace DataLayer.Models.ViewModels
{
    public class ConversationViewModel
    {
        public Guid ConversationId { get; set; }
        public Guid? PublicId { get; set; }
        public string? OtherUsersId { get; set; }
        public string? OtherUsersFullName { get; set; }
        public string? OtherUsername { get; set; }
        public string? OtherUsersProfileImageUrl { get; set; } = "/images/default-profile2.png";
        public List<MessageViewModel> Messages { get; set; }
    }
}
