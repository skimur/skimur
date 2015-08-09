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

        List<Sub> GetSubscribedSubsForUser(Guid userId);

        bool IsUserSubscribedToSub(Guid userId, Guid subId);

        Sub GetRandomSub();

        Sub GetSubByName(string name);

        List<Sub> GetSubsByIds(List<Guid> ids);
        
        bool CanUserModerateSub(Guid userId, Guid subId);

        List<Guid> GetAllModsForSub(Guid subId);
    }

    public enum SubsSortBy
    {
        Relevance,
        Subscribers
    }
}
