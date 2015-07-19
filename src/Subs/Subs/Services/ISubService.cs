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

        List<Sub> GetSubscribedSubsForUser(string userName);

        bool IsUserSubscribedToSub(string userName, string subName);

        Sub GetRandomSub();

        void InsertSub(Sub sub);

        void UpdateSub(Sub sub);

        void DeleteSub(Guid subId);

        void SubscribeToSub(string userName, string subName);

        void UnSubscribeToSub(string userName, string subName);

        Sub GetSubByName(string name);

        List<Sub> GetSubByNames(List<string> names);

        bool CanUserModerateSub(string userName, string subName);

        List<string> GetAllModsForSub(string subName);

        void AddModToSub(string userName, string subName, string addedBy = null);

        void RemoveModFromSub(string userName, string subName);

        void UpdateNumberOfSubscribers(string subName, out ulong totalNumber);
    }
}
