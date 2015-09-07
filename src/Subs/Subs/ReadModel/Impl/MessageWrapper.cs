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

        public MessageWrapper(IMessageDao messageDao, 
            IMembershipService membershipService,
            ISubDao subDao)
        {
            _messageDao = messageDao;
            _membershipService = membershipService;
            _subDao = subDao;
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


            }

            return messages;
        }

        public MessageWrapped Wrap(Guid messageId, User currentUser)
        {
            return Wrap(new List<Guid> { messageId }, currentUser)[0];
        }
    }
}
