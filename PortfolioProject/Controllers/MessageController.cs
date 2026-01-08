using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataLayer.Models.InputModels;
using DataLayer.Models.ViewModels;
using DataLayer.Models;
using DataLayer.Data;
using System;
using System.Linq;
using Castle.Core.Internal;

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

            if (!string.IsNullOrWhiteSpace(username))
            {
                var otherUser = await _userManager.FindByNameAsync(username);
                if (otherUser == null) return NotFound();
                if (otherUser.Id == currentUserId) return Forbid();

                conversationId = await _messages.GetConversationIdBetweenUsersAsync(otherUser.Id, currentUserId);

                //Om ingen konversation finns ännu så skapas en tom vm för start av ny konversation.
                //Den nya konversationen kommer skapas när det första meddelandet skickas.
                if (!conversationId.HasValue || conversationId == Guid.Empty)
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

            //Om en konversation hittades i tidigare if eller om ett conversationId skickades som parameter så hämtas vm
            if (conversationId.HasValue && conversationId != Guid.Empty)
            {
                activeThread = await _messages.GetConversationVmByIdAsync(conversationId.Value, currentUserId);
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
        [HttpPost("send")]
        public async Task<IActionResult> Send(string? username, Guid? conversationId, SendMessageInputModel input)
        {
            var currentUserId = _userManager.GetUserId(User);

            var body = (input.Body ?? "").Trim();
            if (string.IsNullOrWhiteSpace(body))
            {
                TempData["SendError"] = "Meddelandet får inte vara tomt.";
                return RedirectToAction(nameof(Index),
                    !string.IsNullOrWhiteSpace(username)
                        ? new { username }
                        : new { conversationId });

            }

            if (!ModelState.IsValid)
            {
                TempData["SendError"] = "Meddelandet är för långt.";
                return RedirectToAction(nameof(Index),
                    !string.IsNullOrWhiteSpace(username)
                        ? new { username }
                        : new { conversationId });
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                var otherUser = await _userManager.FindByNameAsync(username);
                if (otherUser == null) { return NotFound(); }

                var convoId = await _messages.GetConversationIdBetweenUsersAsync(otherUser.Id, currentUserId);

                var convo = convoId != Guid.Empty
                    ? await _messages.GetConversationByIdAsync(convoId)
                    : await _messages.CreateConversationAsync(otherUser.Id, currentUserId);

                if (convo != null)
                {
                    await _messages.InsertMessageAsync(new Message
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

            if (conversationId is { } cid && cid != Guid.Empty)
            {
                var convo = await _messages.GetConversationByIdAsync(cid);
                if (convo is null) return NotFound();

                if (convo.UserAId != currentUserId && convo.UserBId != currentUserId)
                    return Forbid();

                if (convo != null)
                {
                    await _messages.InsertMessageAsync(new Message
                    {
                        ConversationId = convo.Id,
                        FromUserId = currentUserId,
                        SentAt = DateTime.UtcNow,
                        Body = body
                    });
                }
                return RedirectToAction(nameof(Index), new { conversationId });
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteMessage(Guid messageId, string? username, Guid? conversationId)
        {
            var currentUserId = _userManager.GetUserId(User);

            var deleted = await _messages.MarkReceivedMessageAsDeletedAsync(messageId, currentUserId);

            if (deleted)
            {
                if (!string.IsNullOrWhiteSpace(username))
                    return RedirectToAction(nameof(Index), new { username });

                else if (conversationId.HasValue && conversationId != Guid.Empty)
                    return RedirectToAction(nameof(Index), new { conversationId });
            }

            return RedirectToAction(nameof(Index));

        }

        [AllowAnonymous]
        [HttpGet("start/{username}")]
        public async Task<IActionResult> StartAnonymousThread(string username)
        {
            var isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            if (isLoggedIn)
            {
                return RedirectToAction("Index", new { username });
            }

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
            var isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            if (isLoggedIn)
            {
                return RedirectToAction("Index", new { username });
            }

            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            vm.RecipientUsername = username;
            vm.RecipientFullName = $"{user.FirstName} {user.LastName}";
            vm.RecipientProfileImageUrl = user.ProfileImageUrl;


            if (!ModelState.IsValid)
                return View(vm);

            var convo = await _messages.CreateAnonymousConversationAsync(user.Id, vm.Input.Name);

            var msg = new Message
            {
                ConversationId = convo.Id,
                ToUserId = user.Id,
                AnonymousDisplayName = vm.Input.Name,
                Body = vm.Input.Message
            };

            await _messages.InsertMessageAsync(msg);

            return RedirectToAction("AnonymousThread", new { convo.PublicId });
        }

        [AllowAnonymous]
        [HttpGet("anon/{publicId}")]
        public async Task<IActionResult> AnonymousThread(Guid publicId)
        {
            if (publicId == Guid.Empty) return NotFound();

            var isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            if (isLoggedIn)
            {
                return RedirectToAction("Index");
            }


            var activeThread = await _messages.GetAnonymousConversationVmAsync(publicId);
            if (activeThread == null) return NotFound();

            return View("AnonymousThread", activeThread);
        }

        [AllowAnonymous]
        [HttpPost("anon/{publicId}/send")]
        public async Task<IActionResult> SendAnonymous(Guid publicId, SendMessageInputModel input)
        {
            var isLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;

            if (isLoggedIn)
            {
                return RedirectToAction("Index");
            }


            if (!ModelState.IsValid)
                return RedirectToAction(nameof(AnonymousThread), new { publicId });

            var convo = await _messages.GetAnonymousConversationAsync(publicId);
            if (convo != null)
            {
                await _messages.InsertMessageAsync(new Message
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
