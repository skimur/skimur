using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur;

namespace Subs.ReadModel
{
    public interface ISubDao
    {
        SeekedList<Sub> GetAllSubs(string searchText = null, SubsSortBy sortBy = SubsSortBy.Relevance, int? skip = null, int? take = null);

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
