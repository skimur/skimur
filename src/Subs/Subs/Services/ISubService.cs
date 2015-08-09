using System;
using System.Collections.Generic;
using Skimur;
using Subs.ReadModel;

namespace Subs.Services
{
    public interface ISubService
    {
        SeekedList<Sub> GetAllSubs(string searchText = null, SubsSortBy sortBy = SubsSortBy.Relevance, int? skip = null, int? take = null);

        List<Sub> GetDefaultSubs();

        List<Sub> GetSubscribedSubsForUser(Guid userId);

        bool IsUserSubscribedToSub(Guid userId, Guid subId);

        Sub GetRandomSub();

        void InsertSub(Sub sub);

        void UpdateSub(Sub sub);

        void DeleteSub(Guid subId);

        void SubscribeToSub(Guid userId, Guid subId);

        void UnSubscribeToSub(Guid userId, Guid subId);

        Sub GetSubByName(string name);

        List<Sub> GetSubsByIds(List<Guid> ids);

        bool CanUserModerateSub(Guid userId, Guid subId);

        List<Guid> GetAllModsForSub(Guid subId);

        void AddModToSub(Guid userId, Guid subId, Guid? addedBy = null);

        void RemoveModFromSub(Guid userId, Guid subId);

        void UpdateNumberOfSubscribers(Guid subId, out ulong totalNumber);
    }
}
