using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Logging;
using Infrastructure.Messaging.Handling;
using Infrastructure.Utils;
using Membership;
using Membership.Services;
using Skimur;
using Skimur.Markdown;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker
{
    public class MessagesHandler : ICommandHandlerResponse<SendMessage, SendMessageResponse>
    {
        private readonly ILogger<MessagesHandler> _logger;
        private readonly IMembershipService _membershipService;
        private readonly ISubService _subService;
        private readonly IPermissionService _permissionService;
        private readonly IMessageService _messageService;
        private readonly IMarkdownCompiler _markdownCompiler;

        public MessagesHandler(ILogger<MessagesHandler> logger,
            IMembershipService membershipService,
            ISubService subService,
            IPermissionService permissionService,
            IMessageService messageService,
            IMarkdownCompiler markdownCompiler)
        {
            _logger = logger;
            _membershipService = membershipService;
            _subService = subService;
            _permissionService = permissionService;
            _messageService = messageService;
            _markdownCompiler = markdownCompiler;
        }

        public SendMessageResponse Handle(SendMessage command)
        {
            var response = new SendMessageResponse();

            try
            {
                var author = _membershipService.GetUserById(command.Author);

                if (author == null)
                {
                    response.Error = "No author provided.";
                    return response;
                }

                Sub sendAsSub = null;
                Sub sendingToSub = null;
                User sendingToUser = null;

                if (command.SendAsSub.HasValue)
                {
                    var sub = _subService.GetSubById(command.SendAsSub.Value);

                    if (sub == null)
                    {
                        response.Error = "Invalid sub to send as.";
                        return response;
                    }
                    
                    // the user is trying to send this message as a sub moderator.
                    // let's make sure that this user has this permission for the sub.
                    // TODO: narrow the permission set to be only a "mail" permission.
                    if (!_permissionService.CanUserModerateSub(author, command.SendAsSub.Value))
                    {
                        response.Error = "You are not authorized to send a message as a sub moderator.";
                        return response;
                    }

                    sendAsSub = sub;
                }

                if (string.IsNullOrEmpty(command.To))
                {
                    response.Error = "You must provide a user/sub to send the message to.";
                    return response;
                }
                
                if (command.To.StartsWith("/u/", StringComparison.InvariantCultureIgnoreCase))
                {
                    var userName = command.To.Substring(3);
                    sendingToUser = _membershipService.GetUserByUserName(userName);
                    if (sendingToUser == null)
                    {
                        response.Error = string.Format("No user found with the name {0}.", userName);
                        return response;
                    }
                }
                else if (command.To.StartsWith("/s/", StringComparison.InvariantCultureIgnoreCase))
                {
                    var subName = command.To.Substring(3);
                    sendingToSub = _subService.GetSubByName(subName);
                    if (sendingToSub == null)
                    {
                        response.Error = string.Format("No sub found with the name {0}.", subName);
                        return response;
                    }
                }
                else
                {
                    // maybe they are trying to send a message to a user
                    sendingToUser = _membershipService.GetUserByUserName(command.To);
                    if (sendingToUser == null)
                    {
                        response.Error = string.Format("No user found with the name {0}.", command.To);
                        return response;
                    }
                }

                var message = new Message
                {
                    Id = GuidUtil.NewSequentialId(),
                    DateCreated = Common.CurrentTime(),
                    MessageType = MessageType.Private,
                    ParentId = null,
                    FirstMessage = null,
                    AuthorId = author.Id,
                    AuthorIp = command.AuthorIp,
                    IsNew = true,
                    ToUser = sendingToUser != null ? sendingToUser.Id : (Guid?)null,
                    ToSub = sendingToSub != null ? sendingToSub.Id : (Guid?)null,
                    FromSub = sendAsSub != null ? sendAsSub.Id : (Guid?)null,
                    Subject = command.Subject,
                    Body = command.Body,
                    BodyFormatted = _markdownCompiler.Compile(command.Body),
                };

                _messageService.InsertMessage(message);
            }
            catch (Exception ex)
            {
                _logger.Error("Error sending a message.", ex);
                response.Error = "An unknown error occured.";
            }

            return response;
        }
    }
}
