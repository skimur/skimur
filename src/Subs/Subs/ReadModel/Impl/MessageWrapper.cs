using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel.Impl
{
    public class MessageWrapper : IMessageWrapper
    {
        private readonly IMessageDao _messageDao;

        public MessageWrapper(IMessageDao messageDao)
        {
            _messageDao = messageDao;
        }

        public List<MessageWrapped> Wrap(List<Guid> messageIds)
        {
            return new List<MessageWrapped>();
        }

        public MessageWrapped Wrap(Guid messageId)
        {
            return null;
        }
    }
}
