using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioProject.Data;
using PortfolioProject.Models;
using System.Linq;

namespace PortfolioProject.Controllers
{
    [Authorize]
    [Route("Messages")]
    public class MessageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DatabaseContext _db;
        

        public MessageController(UserManager<User> userManager, DatabaseContext db)
        {
            _userManager = userManager;
            _db = db;       
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            // 1) All messages involving me
            var baseQuery = _db.Messages
                .Where(m => m.FromUserId == currentUserId || m.ToUserId == currentUserId)
                .Select(m => new
                {
                    OtherUserId = m.FromUserId == currentUserId ? m.ToUserId : m.FromUserId,
                    m.SentAt,
                    m.Body,
                    IsMine = m.FromUserId == currentUserId
                });

            // 2) Latest timestamp per conversation
            var latestPerUser = baseQuery
                .GroupBy(x => x.OtherUserId)
                .Select(g => new
                {
                    OtherUserId = g.Key,
                    LastSentAt = g.Max(x => x.SentAt)
                });

            // 3) Get the actual latest message row (tie-safe with SentAt, then body/from)
            var latestMessages = from x in baseQuery
                                 join l in latestPerUser
                                   on new { x.OtherUserId, x.SentAt }
                                   equals new { l.OtherUserId, SentAt = l.LastSentAt }
                                 select x;

            // 4) Join to users and shape the inbox list
            var items = await (from lm in latestMessages
                              join u in _db.Users on lm.OtherUserId equals u.Id
                              orderby lm.SentAt descending
                              select new ConversationListItemViewModel
                              {
                                  OtherUsersId = u.Id,
                                  OtherUsersFullName = $"{u.FirstName}  {u.LastName}",
                                  OtherUsername = u.UserName,
                                  OtherUsersProfileImageUrl = u.ProfileImageUrl,
                                  LastSentAt = lm.SentAt,
                                  LastMessageIsMine = lm.IsMine,
                                  Preview = lm.Body.Length > 50 ? lm.Body.Substring(0, 50) + "…" : lm.Body
                              })
                              .AsNoTracking()
                              .ToListAsync();

            return View(items);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> Conversation(string username)
        {
            var currentUserId = _userManager.GetUserId(User);

            var otherUser = await _userManager.FindByNameAsync(username);
            if (otherUser == null) { return NotFound(); }

            var messages = await _db.Messages
                .Where(m =>
                (m.FromUserId == currentUserId && m.ToUserId == otherUser.Id) ||
                (m.FromUserId == otherUser.Id && m.ToUserId == currentUserId))
                .OrderBy(m => m.SentAt)
                .AsNoTracking()
                .Select(m => new MessageViewModel
                {
                    IsMine = m.FromUserId == currentUserId,
                    SentAt = m.SentAt,
                    Body = m.Body
                })
                .ToListAsync();

            var viewModel = new ConversationViewModel
            {
                OtherUsersId = otherUser.Id,
                OtherUsersFullName = $"{otherUser.FirstName}  {otherUser.LastName}",
                OtherUsersProfileImageUrl = otherUser.ProfileImageUrl,
                Messages = messages
            };

            return View(viewModel);
        }

    }
}
