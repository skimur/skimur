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
        SeekedList<Guid> GetAllSubs(string searchText = null, SubsSortBy sortBy = SubsSortBy.Relevance, int? skip = null, int? take = null);

        List<Guid> GetDefaultSubs();

        List<Guid> GetSubscribedSubsForUser(Guid userId);

        bool IsUserSubscribedToSub(Guid userId, Guid subId);

        Guid? GetRandomSub();

        Sub GetSubByName(string name);

        List<Sub> GetSubsByIds(List<Guid> ids);

        Sub GetSubById(Guid id);

        bool CanUserModerateSub(Guid userId, Guid subId);

        List<Guid> GetAllModsForSub(Guid subId);
    }

    public enum SubsSortBy
    {
        Relevance,
        Subscribers
    }
}
