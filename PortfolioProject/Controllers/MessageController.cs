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
        private readonly IMessagesService _messages;


        public MessageController(UserManager<User> userManager, IMessagesService messages)
        {
            _userManager = userManager;
            _messages = messages;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? username)
        {
            var currentUserId = _userManager.GetUserId(User);

            var items = await _messages.GetInboxAsync(currentUserId);

            ConversationViewModel? active = null;

            // === HÖGER: Om username är vald, bygg samma som Conversation() ===
            if (!string.IsNullOrWhiteSpace(username))
            {
                var otherUser = await _userManager.FindByNameAsync(username);
                if (otherUser == null) return NotFound();
                if (otherUser.Id == currentUserId) return Forbid();

                active = await _messages.GetConversationViewModelAsync(otherUser.Id, currentUserId!);

                if (active is null)
                {
                    active = new ConversationViewModel
                    {
                        OtherUsersId = otherUser.Id,
                        OtherUsersFullName = $"{otherUser.FirstName} {otherUser.LastName}",
                        OtherUsername = otherUser.UserName,
                        OtherUsersProfileImageUrl = otherUser.ProfileImageUrl,
                        Messages = new List<MessageViewModel>()
                    };
                }

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


            var convo = await _messages.EnsureConversationForSendAsync(otherUser.Id, currentUserId);
            if (convo != null)
            {
                await _messages.InsertMessage(new Message
                {
                    ConversationId = convo.Id,
                    FromUserId = currentUserId,
                    ToUserId = otherUser.Id,
                    SentAt = DateTime.UtcNow,
                    Body = body
                });
            }
            return RedirectToAction(nameof(Index), new { username });



        }
    }
}
