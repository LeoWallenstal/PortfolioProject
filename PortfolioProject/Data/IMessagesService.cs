using PortfolioProject.Models;

namespace PortfolioProject.Data
{
    public interface IMessagesService
    {


        Task<List<ConversationListItemViewModel>> GetInboxAsync(string currentUserId);
        Task<Guid> GetConversationIdBetweenUsersAsync(string otherUserId, string currentUserId);
        Task<ConversationViewModel?> GetConversationVmByIdAsync(Guid conversationId, string currentUserId);
        Task<Conversation> EnsureConversationForSendAsync(string otherUserId, string currentUserId);
        Task<Conversation?> GetConversationByIdAsync(Guid conversationId);
        Task InsertMessage(Message msg);
        Task<int> GetTotalUnreadAsync(string userId);
        Task<Conversation> CreateAnonymousConversationAsync(string recipientUserId, string displayName);
        Task<ConversationViewModel?> GetAnonymousConversationVmAsync(Guid publicId);
        Task<Conversation?> GetAnonymousConversationAsync(Guid publicId);
    }
}
