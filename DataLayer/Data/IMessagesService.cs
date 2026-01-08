using DataLayer.Models.ViewModels;
using DataLayer.Models;

namespace DataLayer.Data
{
    public interface IMessagesService
    {

        Task<Conversation> CreateConversationAsync(string otherUserId, string currentUserId);
        Task<List<ConversationListItemViewModel>> GetInboxAsync(string currentUserId);
        Task<Guid> GetConversationIdBetweenUsersAsync(string otherUserId, string currentUserId);
        Task<ConversationViewModel?> GetConversationVmByIdAsync(Guid conversationId, string currentUserId);
        Task<Conversation?> GetConversationByIdAsync(Guid conversationId);
        Task InsertMessageAsync(Message msg);
        Task<bool> MarkReceivedMessageAsDeletedAsync(Guid messageId, string currentUserId);
        Task<int> GetTotalUnreadAsync(string userId);
        Task<Conversation> CreateAnonymousConversationAsync(string recipientUserId, string displayName);
        Task<ConversationViewModel?> GetAnonymousConversationVmAsync(Guid publicId);
        Task<Conversation?> GetAnonymousConversationAsync(Guid publicId);
    }
}
