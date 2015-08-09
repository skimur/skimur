using System;
using System.Collections.Generic;

namespace Skimur.Web
{
    public interface IContextService
    {
        List<Guid> GetSubscribedSubIds();

        bool IsSubcribedToSub(Guid subId);
    }
}
