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
using Subs.ReadModel;
using Subs.Services;

namespace Subs.Worker
{
    public class MessagesHandler : 
        ICommandHandlerResponse<SendMessage, SendMessageResponse>,
        ICommandHandlerResponse<ReplyMessage, ReplyMessageResponse>,
        ICommandHandler<MarkMessagesAsRead>,
        ICommandHandler<MarkMessagesAsUnread>
    {
        private readonly ILogger<MessagesHandler> _logger;
        private readonly IMembershipService _membershipService;
        private readonly ISubService _subService;
        private readonly IPermissionService _permissionService;
        private readonly IMessageService _messageService;
        private readonly IMarkdownCompiler _markdownCompiler;
        private readonly IPermissionDao _permissionDao;

        public MessagesHandler(ILogger<MessagesHandler> logger,
            IMembershipService membershipService,
            ISubService subService,
            IPermissionService permissionService,
            IMessageService messageService,
            IMarkdownCompiler markdownCompiler,
            IPermissionDao permissionDao)
        {
            _logger = logger;
            _membershipService = membershipService;
            _subService = subService;
            _permissionService = permissionService;
            _messageService = messageService;
            _markdownCompiler = markdownCompiler;
            _permissionDao = permissionDao;
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

                response.MessageId = message.Id;
            }
            catch (Exception ex)
            {
                _logger.Error("Error sending a message.", ex);
                response.Error = "An unknown error occured.";
            }

            return response;
        }

        public ReplyMessageResponse Handle(ReplyMessage command)
        {
            var response = new ReplyMessageResponse();

            try
            {
                var author = _membershipService.GetUserById(command.Author);

                if (author == null)
                {
                    response.Error = "No author provided.";
                    return response;
                }

                var replyToMessage = _messageService.GetMessageById(command.ReplyToMessageId);

                if (replyToMessage == null)
                {
                    response.Error = "Invalid message.";
                    return response;
                }

                if (replyToMessage.MessageType != MessageType.Private)
                    throw new Exception("An attempt was made to reply to a message that wasn't a private message.");

                // let's determine if the user has adequate permissions to reply to this message
                if (replyToMessage.ToUser.HasValue && replyToMessage.ToUser.Value == author.Id)
                {
                    // the user is replying to a message that was sent to him/her.
                }else if (replyToMessage.ToSub.HasValue &&
                          // TODO: use a narrower set of permissions
                          _permissionService.CanUserModerateSub(author, replyToMessage.ToSub.Value))
                {
                    // the user is replying to a sub message, and the user is allowed to moderate this sub!
                }
                else
                {
                    // the current user is not allowed to reply to this message.
                    response.Error = "Not authorized.";
                    return response;
                }

                Sub sendingToSub = null;
                User sendingToUser = null;

                if (replyToMessage.FromSub.HasValue)
                {
                    sendingToSub = _subService.GetSubById(replyToMessage.FromSub.Value);
                    if (sendingToSub == null) throw new Exception("Couldn't get the sub");
                }
                else
                {
                    sendingToUser = _membershipService.GetUserById(replyToMessage.AuthorId);
                    if (sendingToUser == null) throw new Exception("Couldn't get the user");
                }

                Sub sendAsSub = null;

                if (replyToMessage.ToSub.HasValue)
                {
                    // the user originally sent this message to a sub, so reply as a sub
                    sendAsSub = _subService.GetSubById(replyToMessage.ToSub.Value);
                    if(sendAsSub == null) throw new Exception("Couldn't get the sub");
                } 

                var subject = replyToMessage.Subject;

                if (!subject.StartsWith("re: "))
                    subject = "re: " + subject;

                var message = new Message
                {
                    Id = GuidUtil.NewSequentialId(),
                    DateCreated = Common.CurrentTime(),
                    MessageType = MessageType.Private,
                    ParentId = replyToMessage.Id,
                    FirstMessage = replyToMessage.FirstMessage.HasValue ? replyToMessage.FirstMessage.Value : replyToMessage.Id,
                    AuthorId = author.Id,
                    AuthorIp = command.AuthorIp,
                    IsNew = true,
                    ToUser = sendingToUser != null ? sendingToUser.Id : (Guid?)null,
                    ToSub = sendingToSub != null ? sendingToSub.Id : (Guid?)null,
                    FromSub = sendAsSub != null ? sendAsSub.Id : (Guid?)null,
                    Subject = subject,
                    Body = command.Body,
                    BodyFormatted = _markdownCompiler.Compile(command.Body),
                };

                _messageService.InsertMessage(message);

                response.MessageId = message.Id;
            }
            catch (Exception ex)
            {
                _logger.Error("Error sending a message.", ex);
                response.Error = "An unknown error occured.";
            }

            return response;
        }

        public void Handle(MarkMessagesAsRead command)
        {
            if (command.Messages == null || command.Messages.Count == 0)
                return;

            var user = _membershipService.GetUserById(command.UserId);
            if (user == null) return;

            var messages = _messageService.GetMessagesByIds(command.Messages);
            var subs = new List<Guid>();

            foreach (var message in messages)
            {
                if (message.ToSub.HasValue && !subs.Contains(message.ToSub.Value))
                    subs.Add(message.ToSub.Value);
            }

            var subsCanModerate = new List<Guid>();
            foreach (var sub in subs)
            {
                // TODO: check for a narrower set of permissions
                if (_permissionDao.CanUserModerateSub(user, sub))
                    subsCanModerate.Add(sub);
            }

            var messagesToMarkAsRead = new List<Guid>();
            foreach (var message in messages)
            {
                if (message.ToUser.HasValue && message.ToUser == user.Id)
                {
                    // this message was sent to this user
                    messagesToMarkAsRead.Add(message.Id);
                }else if (message.ToSub.HasValue && subsCanModerate.Contains(message.ToSub.Value))
                {
                    // this message was sent to a sub that the user is a moderator of
                    messagesToMarkAsRead.Add(message.Id);
                }
            }

            if (messagesToMarkAsRead.Count > 0)
                _messageService.MarkMessagesAsRead(messagesToMarkAsRead);
        }

        public void Handle(MarkMessagesAsUnread command)
        {
            if (command.Messages == null || command.Messages.Count == 0)
                return;

            var user = _membershipService.GetUserById(command.UserId);
            if (user == null) return;

            var messages = _messageService.GetMessagesByIds(command.Messages);
            var subs = new List<Guid>();

            foreach (var message in messages)
            {
                if (message.ToSub.HasValue && !subs.Contains(message.ToSub.Value))
                    subs.Add(message.ToSub.Value);
            }

            var subsCanModerate = new List<Guid>();
            foreach (var sub in subs)
            {
                // TODO: check for a narrower set of permissions
                if (_permissionDao.CanUserModerateSub(user, sub))
                    subsCanModerate.Add(sub);
            }

            var messagesToMarkAsRead = new List<Guid>();
            foreach (var message in messages)
            {
                if (message.ToUser.HasValue && message.ToUser == user.Id)
                {
                    // this message was sent to this user
                    messagesToMarkAsRead.Add(message.Id);
                }
                else if (message.ToSub.HasValue && subsCanModerate.Contains(message.ToSub.Value))
                {
                    // this message was sent to a sub that the user is a moderator of
                    messagesToMarkAsRead.Add(message.Id);
                }
            }

            if (messagesToMarkAsRead.Count > 0)
                _messageService.MarkMessagesAsUnread(messagesToMarkAsRead);
        }
    }
}
