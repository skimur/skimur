using System;
using System.Collections.Generic;

namespace Skimur.App.ReadModel
{
    public interface IMessageWrapper
    {
        List<MessageWrapped> Wrap(List<Guid> messageIds, User currentUser);

        MessageWrapped Wrap(Guid messageId, User currentUser);
    }
}
