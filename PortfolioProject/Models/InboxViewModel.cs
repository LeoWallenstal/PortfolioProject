namespace PortfolioProject.Models
{
    public class InboxViewModel
    {
        public List<ConversationListItemViewModel> ConversationList { get; set; } = new();
        public ConversationViewModel? Conversation { get; set; }
    }
}
