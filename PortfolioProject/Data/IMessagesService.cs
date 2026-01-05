using PortfolioProject.Models;

namespace PortfolioProject.Data
{
    public interface IMessagesService
    {


        Task<List<ConversationListItemViewModel>> GetInboxAsync(string currentUserId);
        Task<ConversationViewModel?> GetConversationViewModelAsync(string otherUserId, string currentUserId);
        Task<Conversation> EnsureConversationForSendAsync(string otherUserId, string currentUserId);
        Task InsertMessage(Message msg);

        Task<int> GetTotalUnreadAsync(string userId);
    }
}
