using System.Collections.Generic;

namespace Skimur.Web
{
    public interface IContextService
    {
        List<string> GetSubscribedSubNames();

        bool IsSubcribedToSub(string subName);
    }
}
