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

        [HttpGet("")]
        public async Task<IActionResult> Index(string? username)
        {
            var currentUserId = _userManager.GetUserId(User);

            // === VÄNSTER: Samma som Index() ===
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
                                   Preview = lm.Body.Length > 25 ? lm.Body.Substring(0, 25).Trim() + "…" : lm.Body
                               })
                              .AsNoTracking()
                              .ToListAsync();
            ConversationViewModel? active = null;

            // === HÖGER: Om username är vald, bygg samma som Conversation() ===
            if (!string.IsNullOrWhiteSpace(username))
            {
                var otherUser = await _userManager.FindByNameAsync(username);
                if (otherUser == null) return NotFound();
                if (otherUser.Id == currentUserId) return Forbid();

                var unread = await _db.Messages
                    .Where(m =>
                        !m.IsRead &&
                        m.ToUserId == currentUserId &&
                        m.FromUserId == otherUser.Id)
                    .ToListAsync();

                if (unread.Any())
                {
                    foreach (var msg in unread)
                        msg.IsRead = true;

                    await _db.SaveChangesAsync();
                }

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
                        Body = m.Body,
                        IsRead = m.IsRead
                    })
                    .ToListAsync();

                active = new ConversationViewModel
                {
                    OtherUsersId = otherUser.Id,
                    OtherUsersFullName = $"{otherUser.FirstName}  {otherUser.LastName}",
                    OtherUsername = otherUser.UserName,
                    OtherUsersProfileImageUrl = otherUser.ProfileImageUrl,
                    Messages = messages
                };
            }

            var vm = new InboxViewModel
            {
                ConversationList = items,
                Conversation = active
            };


            var isFetch = Request.Headers["X-Requested-With"] == "fetch";

            if (isFetch)
            {          
                if (vm.Conversation is null)
                    return PartialView("_ConversationThreadEmpty");

                return PartialView("_ConversationThread", vm.Conversation);
            }

            return View(vm);
        }

        [HttpPost("{username}/send")]
        public async Task<IActionResult> Send(string username, SendMessageInputModel input)
        {
            var currentUserId = _userManager.GetUserId(User);

            var otherUser = await _userManager.FindByNameAsync(username);
            if (otherUser == null) { return NotFound(); }

            var body = (input.Body ?? "").Trim();
            if (string.IsNullOrWhiteSpace(body))
            {
                TempData["SendError"] = "Meddelandet får inte vara tomt.";
                return RedirectToAction(nameof(Index), new { username });
            }

            if (!ModelState.IsValid)
            {
                TempData["SendError"] = "Meddelandet är för långt.";
                return RedirectToAction(nameof(Index), new { username });
            }

            _db.Messages.Add(new Message
            {
                FromUserId = currentUserId,
                ToUserId = otherUser.Id,
                SentAt = DateTime.UtcNow,
                Body = body
            });

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { username });

        }

    }
}
