using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;
using Membership.Services;

namespace Subs.ReadModel.Impl
{
    public class MessageWrapper : IMessageWrapper
    {
        private readonly IMessageDao _messageDao;
        private readonly IMembershipService _membershipService;
        private readonly ISubDao _subDao;
        private readonly IPermissionDao _permissionDao;

        public MessageWrapper(IMessageDao messageDao, 
            IMembershipService membershipService,
            ISubDao subDao,
            IPermissionDao permissionDao)
        {
            _messageDao = messageDao;
            _membershipService = membershipService;
            _subDao = subDao;
            _permissionDao = permissionDao;
        }

        public List<MessageWrapped> Wrap(List<Guid> messageIds, User currentUser)
        {
            if(currentUser == null) throw new Exception("You must provide a user.");

            var messages = new List<MessageWrapped>();

            foreach (var messageId in messageIds)
            {
                var message = _messageDao.GetMessageById(messageId);
                if (message != null)
                    messages.Add(new MessageWrapped(message));
            }

            var users = new Dictionary<Guid, User>();
            var subs = new Dictionary<Guid, Sub>();

            foreach (var message in messages)
            {
                if (!users.ContainsKey(message.Message.AuthorId))
                    users.Add(message.Message.AuthorId, null);
                if (message.Message.ToUser.HasValue && !users.ContainsKey(message.Message.ToUser.Value))
                    users.Add(message.Message.ToUser.Value, null);
                if (message.Message.FromSub.HasValue && !subs.ContainsKey(message.Message.FromSub.Value))
                    subs.Add(message.Message.FromSub.Value, null);
                if (message.Message.ToSub.HasValue && !subs.ContainsKey(message.Message.ToSub.Value))
                    subs.Add(message.Message.ToSub.Value, null);
            }

            var subsCanModerate = new HashSet<Guid>();
            foreach (var sub in subs.Keys)
            {
                if (_permissionDao.CanUserModerateSub(currentUser, sub))
                    subsCanModerate.Add(sub);
            }

            foreach (var userId in users.Keys.ToList())
            {
                users[userId] = _membershipService.GetUserById(userId);
            }

            foreach (var subId in subs.Keys.ToList())
            {
                subs[subId] = _subDao.GetSubById(subId);
            }

            foreach (var message in messages)
            {
                message.Author = users[message.Message.AuthorId];
                message.FromSub = message.Message.FromSub.HasValue ? subs[message.Message.FromSub.Value] : null;
                message.ToUser = message.Message.ToUser.HasValue ? users[message.Message.ToUser.Value] : null;
                message.ToSub = message.Message.ToSub.HasValue ? subs[message.Message.ToSub.Value] : null;

                if (message.ToUser != null && message.ToUser.Id == currentUser.Id)
                {
                    // this was a message to the current user, so the current user can reply to it.
                    message.CanReply = true;
                }
                else if(message.ToSub != null && subsCanModerate.Contains(message.ToSub.Id))
                {
                    // this message was sent to a sub, and this user is a moderator with the correct permissions to reply.
                    message.CanReply = true;
                }

                if (message.Author.Id == currentUser.Id)
                {
                    message.UserIsSender = true;
                }
                else if(message.ToUser != null && message.ToUser.Id == currentUser.Id)
                {
                    message.UserIsRecipiant = true;
                }else if (message.ToSub != null && subsCanModerate.Contains(message.ToSub.Id))
                {
                    message.UserIsRecipiant = true;
                }
            }

            return messages;
        }

        public MessageWrapped Wrap(Guid messageId, User currentUser)
        {
            return Wrap(new List<Guid> { messageId }, currentUser)[0];
        }
    }
}
