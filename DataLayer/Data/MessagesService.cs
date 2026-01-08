using DataLayer.Models;
using DataLayer.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace DataLayer.Data
{
    public class MessagesService : IMessagesService
    {
        private readonly DatabaseContext _dbContext;
        private readonly UserManager<User> _userManager;


        public MessagesService(DatabaseContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<Conversation> CreateConversationAsync(string otherUserId, string currentUserId)
        {
            var convo = new Conversation
            {
                Id = Guid.NewGuid(),
                UserAId = currentUserId!,
                UserBId = otherUserId
            };

            _dbContext.Conversations.Add(convo);
            await _dbContext.SaveChangesAsync();

            return convo;
        }

        public async Task<List<ConversationListItemViewModel>> GetInboxAsync(string currentUserId)
        {
            var convos = await _dbContext.Conversations
                .Where(c => c.UserAId == currentUserId || c.UserBId == currentUserId)
                .AsNoTracking()
                .ToListAsync();

            var convoIds = convos.Select(c => c.Id).ToList();

            var latestMessages = await _dbContext.Messages
                .Where(m => convoIds.Contains(m.ConversationId))
                .Where(m => m.ToUserId != currentUserId || !m.IsDeletedByReceiver)
                .GroupBy(m => m.ConversationId)
                .Select(g => g
                    .OrderByDescending(m => m.SentAt)
                    .ThenByDescending(m => m.MessageId)
                    .Select(m => new
                    {
                        m.ConversationId,
                        m.SentAt,
                        m.Body,
                        IsMine = m.FromUserId == currentUserId,
                        m.IsDeletedByReceiver
                    })
                    .FirstOrDefault()!)
                .ToListAsync();


            var otherUserIds = convos
                .Select(c => c.UserAId == currentUserId ? c.UserBId : c.UserAId)
                .Where(id => id != null)
                .Distinct()
                .ToList()!;

            var users = await _dbContext.Users
                .Where(u => otherUserIds.Contains(u.Id))
                .AsNoTracking()
                .ToDictionaryAsync(u => u.Id);

            var unreadMap = await _dbContext.Messages
                .Where(m => convoIds.Contains(m.ConversationId))
                .Where(m => m.FromUserId != currentUserId)
                .Where(m => !m.IsRead)
                .GroupBy(m => m.ConversationId)
                .Select(g => new
                {
                    ConversationId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.ConversationId, x => x.Count);




            var items = latestMessages
            .OrderByDescending(x => x.SentAt)
            .Select(msg =>
            {
                var convo = convos.First(c => c.Id == msg.ConversationId);
                var preview = (msg.Body ?? "");
                preview = preview.Length > 25 ? preview.Substring(0, 25).Trim() + "…" : preview;


                var convoListItem = new ConversationListItemViewModel
                {
                    ConversationId = msg.ConversationId,
                    LastSentAt = msg.SentAt,
                    LastMessageIsMine = msg.IsMine,
                    Preview = preview,
                    UnreadCount = unreadMap.TryGetValue(msg.ConversationId, out var cnt) ? cnt : 0
                };

                if (convo.IsAnonymous)
                {
                    convoListItem.OtherUsersFullName = $"{convo.AnonymousDisplayName}";
                }
                else
                {
                    var otherId = convo.UserAId == currentUserId ? convo.UserBId : convo.UserAId;
                    users.TryGetValue(otherId!, out var other);

                    convoListItem.OtherUsersFullName = other.IsActive ? $"{other.FirstName} {other.LastName}" : "Avaktiverad användare";
                    convoListItem.OtherUsersId = other.Id;

                    if (other.IsActive)
                    {
                        convoListItem.OtherUsername = other.UserName;
                        convoListItem.OtherUsersProfileImageUrl = other.ProfileImageUrl;
                    }

                }
                return convoListItem;
            })
            .ToList();

            return items;
        }

        public async Task<Guid> GetConversationIdBetweenUsersAsync(string otherUserId, string currentUserId)
        {
            return await _dbContext.Conversations
                    .Where(c => (c.UserAId == currentUserId && c.UserBId == otherUserId) ||
                                (c.UserAId == otherUserId && c.UserBId == currentUserId))
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();
        }

        public async Task<ConversationViewModel?> GetConversationVmByIdAsync(Guid conversationId, string currentUserId)
        {
            var convo = await _dbContext.Conversations
                .AsNoTracking()
                .Where(c => c.Id == conversationId)
                .Where(c => c.UserAId == currentUserId || c.UserBId == currentUserId)
                .Select(c => new
                {
                    c.Id,
                    c.UserAId,
                    c.UserBId,
                    c.IsAnonymous,
                    c.AnonymousDisplayName,
                    Messages = c.Messages
                        .OrderBy(m => m.SentAt)
                        .Select(m => new MessageViewModel
                        {
                            MessageId = m.MessageId,
                            IsMine = m.FromUserId == currentUserId,
                            SentAt = m.SentAt,
                            Body = m.Body,
                            IsRead = m.IsRead,
                            IsDeletedByReceiver = m.IsDeletedByReceiver
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();


            if (convo is null)
                return null;

            var unreadMessages = await _dbContext.Messages
                .Where(m => m.ConversationId == conversationId)
                .Where(m => m.ToUserId == currentUserId)
                .Where(m => !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                    msg.IsRead = true;

                await _dbContext.SaveChangesAsync();
            }

            var convoVm = new ConversationViewModel
            {
                ConversationId = conversationId,
                Messages = convo.Messages
            };

            if (convo.IsAnonymous)
            {
                convoVm.OtherUsersFullName = convo.AnonymousDisplayName;

            }
            else
            {
                var otherUserId = currentUserId == convo.UserAId ? convo.UserBId : convo.UserAId;
                var otherUser = await _userManager.FindByIdAsync(otherUserId);

                if (otherUser is null)
                    return null;

                convoVm.OtherUsersFullName = otherUser.IsActive ? $"{otherUser.FirstName} {otherUser.LastName}" : "Avaktiverad användare";
                convoVm.OtherUsersId = otherUser.Id;

                if (otherUser.IsActive) {
                    convoVm.OtherUsername = otherUser.UserName;
                    convoVm.OtherUsersProfileImageUrl = otherUser.ProfileImageUrl;
                }
            }
            return convoVm;
        }

        public async Task<Conversation?> GetConversationByIdAsync(Guid conversationId)
        {
            return await _dbContext.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId);
        }


        public async Task InsertMessageAsync(Message msg)
        {
            _dbContext.Messages.Add(msg);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> MarkReceivedMessageAsDeletedAsync(Guid messageId, string currentUserId)
        {
            var msg = await _dbContext.Messages
                .FirstOrDefaultAsync(m => m.MessageId == messageId);

            if (msg == null) return false;

            if (msg.ToUserId != currentUserId) return false;

            msg.IsDeletedByReceiver = true;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetTotalUnreadAsync(string userId)
        {
            return await _dbContext.Messages.CountAsync(m => m.ToUserId == userId && !m.IsRead);
        }

        public async Task<Conversation> CreateAnonymousConversationAsync(string recipientUserId, string displayName)
        {
            var convo = new Conversation
            {
                IsAnonymous = true,
                PublicId = Guid.NewGuid(),
                AnonymousDisplayName = displayName,
                UserAId = recipientUserId
            };

            _dbContext.Conversations.Add(convo);
            await _dbContext.SaveChangesAsync();
            return convo;
        }

        public async Task<ConversationViewModel?> GetAnonymousConversationVmAsync(Guid publicId)
        {
            var convo = await _dbContext.Conversations
                .AsNoTracking()
                .Where(c => c.IsAnonymous && c.PublicId == publicId)
                .Select(c => new
                {
                    c.Id,
                    c.AnonymousDisplayName,
                    c.AnonymousPasswordHash,
                    RecipientUserId = c.UserAId,
                    Messages = c.Messages
                        .OrderBy(m => m.SentAt)
                        .Select(m => new MessageViewModel
                        {
                            MessageId = m.MessageId,
                            IsMine = m.FromUserId == null,
                            SentAt = m.SentAt,
                            Body = m.Body,
                            IsRead = m.IsRead
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (convo == null) { return null; }

            var recipient = await _userManager.FindByIdAsync(convo.RecipientUserId);

            var unreadMessages = await _dbContext.Messages
                .Where(m => m.ConversationId == convo.Id)
                .Where(m => m.ToUserId == null)
                .Where(m => !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages)
                    msg.IsRead = true;

                await _dbContext.SaveChangesAsync();
            }

            return new ConversationViewModel
            {
                ConversationId = convo.Id,
                PublicId = publicId,
                OtherUsersId = recipient?.Id,
                OtherUsersFullName = $"{recipient?.FirstName} {recipient?.LastName}",
                OtherUsername = recipient?.UserName,
                OtherUsersProfileImageUrl = recipient?.ProfileImageUrl ?? "/images/default-profile2.png",
                Messages = convo.Messages
            };
        }

        public async Task<Conversation?> GetAnonymousConversationAsync(Guid publicId)
        {
            var convo = await _dbContext.Conversations
                .AsNoTracking()
                .Where(c => c.IsAnonymous && c.PublicId == publicId)
                .FirstOrDefaultAsync();

            return convo;
        }
    }
}
