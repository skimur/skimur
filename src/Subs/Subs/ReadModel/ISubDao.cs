using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel
{
    public interface ISubDao
    {
        List<Sub> GetAllSubs(string searchText = null, SubsSortBy sortBy = SubsSortBy.Relevance);

        List<Sub> GetDefaultSubs();

        List<Sub> GetSubscribedSubsForUser(string userName);

        bool IsUserSubscribedToSub(string userName, string subName);

        Sub GetRandomSub();

        Sub GetSubByName(string name);

        List<Sub> GetSubByNames(List<string> names);

        bool CanUserModerateSub(string userName, string subName);

        List<string> GetAllModsForSub(string subName);
    }

    public enum SubsSortBy
    {
        Relevance,
        Subscribers
    }
}
