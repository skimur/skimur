using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.Services
{
    public interface IContextService
    {
        List<Guid> GetSubscribedSubIds();

        bool IsSubcribedToSub(Guid subId);
    }
}
