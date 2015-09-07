using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;

namespace Subs.ReadModel
{
    public class MessageWrapped
    {
        public MessageWrapped(Message message)
        {
            Message = message;
        }

        public Message Message { get; private set; }
        
        public User Author { get; set; }

        public Sub FromSub { get; set; }

        public User ToUser { get; set; }

        public Sub ToSub { get; set; }
    }
}
