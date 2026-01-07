using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioProject.Data;
using PortfolioProject.Models;
using PortfolioProject.Models.InputModels;
using PortfolioProject.Models.ViewModels;
using System.Linq;

namespace PortfolioProject.Controllers
{

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

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> Index(string? username, Guid? conversationId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var items = await _messages.GetInboxAsync(currentUserId);

            ConversationViewModel? activeThread = null;

            if (conversationId.HasValue)
            {
                activeThread = await _messages.GetConversationVmByIdAsync(conversationId.Value, currentUserId);
            }
            else if (!string.IsNullOrWhiteSpace(username))
            {
                var otherUser = await _userManager.FindByNameAsync(username);
                if (otherUser == null) return NotFound();
                if (otherUser.Id == currentUserId) return Forbid();

                activeThread = await _messages.GetConversationVmAsync(otherUser.Id, currentUserId!);

                if (activeThread is null)
                {
                    activeThread = new ConversationViewModel
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
                Conversation = activeThread
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

        [Authorize]
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

        [Authorize]
        [HttpPost("{conversationId:guid}/send")]
        public async Task<IActionResult> Send(Guid conversationId, SendMessageInputModel input)
        {
            var currentUserId = _userManager.GetUserId(User);

            var body = (input.Body ?? "").Trim();
            if (string.IsNullOrWhiteSpace(body))
            {
                TempData["SendError"] = "Meddelandet får inte vara tomt.";
                return RedirectToAction(nameof(Index), new { conversationId });
            }

            if (!ModelState.IsValid)
            {
                TempData["SendError"] = "Meddelandet är för långt.";
                return RedirectToAction(nameof(Index), new { conversationId });
            }


            var convo = await _messages.GetConversationByIdAsync(conversationId, currentUserId);
            if (convo != null)
            {
                await _messages.InsertMessage(new Message
                {
                    ConversationId = convo.Id,
                    FromUserId = currentUserId,
                    SentAt = DateTime.UtcNow,
                    Body = body
                });
            }
            return RedirectToAction(nameof(Index), new { conversationId });
        }



        [AllowAnonymous]
        [HttpGet("start/{username}")]
        public async Task<IActionResult> StartAnonymousThread(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            var vm = new StartAnonymousThreadViewModel
            {
                RecipientUsername = username,
                RecipientFullName = $"{user.FirstName} {user.LastName}",
                RecipientProfileImageUrl = user.ProfileImageUrl
            };

            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost("start/{username}")]
        public async Task<IActionResult> StartAnonymousThread(string username, StartAnonymousThreadViewModel vm)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            vm.RecipientUsername = username;
            vm.RecipientFullName = $"{user.FirstName} {user.LastName}";
            vm.RecipientProfileImageUrl = user.ProfileImageUrl;


            if (!ModelState.IsValid)
                return View(vm);

            // success: create conversation, save message...
            var convo = await _messages.CreateAnonymousConversationAsync(user.Id, vm.Input.Name);

            var msg = new Message
            {
                ConversationId = convo.Id,
                ToUserId = user.Id,
                AnonymousDisplayName = vm.Input.Name,
                Body = vm.Input.Message
            };

            await _messages.InsertMessage(msg);

            return RedirectToAction("AnonymousThread", new { convo.PublicId });
        }

        [AllowAnonymous]
        [HttpGet("anon/{publicId}")]
        public async Task<IActionResult> AnonymousThread(Guid publicId)
        {
            if (publicId == Guid.Empty) return NotFound();

            var activeThread = await _messages.GetAnonymousConversationVmAsync(publicId);
            if (activeThread == null) return NotFound();

            return View("AnonymousThread", activeThread);
        }

        [AllowAnonymous]
        [HttpPost("anon/{publicId}/send")]
        public async Task<IActionResult> SendAnonymous(Guid publicId, SendMessageInputModel input)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(AnonymousThread), new { publicId });

            var convo = await _messages.GetAnonymousConversationAsync(publicId);
            if (convo != null)
            {
                await _messages.InsertMessage(new Message
                {
                    ConversationId = convo.Id,
                    AnonymousDisplayName = convo.AnonymousDisplayName,
                    ToUserId = convo.UserAId,
                    SentAt = DateTime.UtcNow,
                    Body = input.Body
                });
            }
            // 4) Redirect back to thread
            return RedirectToAction(nameof(AnonymousThread), new { publicId });




        }


    }
}
