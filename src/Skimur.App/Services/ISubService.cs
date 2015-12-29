using System;
using System.Collections.Generic;
using Skimur.App.ReadModel;

namespace Skimur.App.Services
{
    public interface ISubService
    {
        SeekedList<Guid> GetAllSubs(string searchText = null,
            SubsSortBy sortBy = SubsSortBy.Relevance,
            bool? nsfw = null,
            int? skip = null,
            int? take = null);

        List<Guid> GetDefaultSubs();

        List<Guid> GetSubscribedSubsForUser(Guid userId);

        bool IsUserSubscribedToSub(Guid userId, Guid subId);

        Guid? GetRandomSub(bool? nsfw = null);

        void InsertSub(Sub sub);

        void UpdateSub(Sub sub);

        void DeleteSub(Guid subId);

        void SubscribeToSub(Guid userId, Guid subId);

        void UnSubscribeToSub(Guid userId, Guid subId);

        Sub GetSubByName(string name);

        List<Sub> GetSubsByIds(List<Guid> ids);

        Sub GetSubById(Guid id);
        
        void UpdateNumberOfSubscribers(Guid subId, out ulong totalNumber);
    }
}
