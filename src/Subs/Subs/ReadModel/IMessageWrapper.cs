using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership;

namespace Subs.ReadModel
{
    public interface IMessageWrapper
    {
        List<MessageWrapped> Wrap(List<Guid> messageIds);

        MessageWrapped Wrap(Guid messageId);
    }
}
