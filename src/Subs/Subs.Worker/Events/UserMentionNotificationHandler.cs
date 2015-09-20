using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging.Handling;
using Subs.Events;

namespace Subs.Worker.Events
{
    public class UserMentionNotificationHandler :
        IEventHandler<UsersMentioned>,
        IEventHandler<UsersUnmentioned>
    {
        public void Handle(UsersUnmentioned @event)
        {

        }

        public void Handle(UsersMentioned @event)
        {

        }
    }
}
