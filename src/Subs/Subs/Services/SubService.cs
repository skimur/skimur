using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Utils;
using ServiceStack.OrmLite;
using Skimur;
using Subs.ReadModel;

namespace Subs.Services
{
    public class SubService : ISubService
    {
        private IDbConnectionProvider _conn;
        private readonly IMapper _mapper;

        public SubService(IDbConnectionProvider conn, IMapper mapper)
        {
            _conn = conn;
            _mapper = mapper;
        }

        public List<Sub> GetAllSubs(string searchText = null, SubsSortBy sortBy = SubsSortBy.Relevance)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Sub>();
                if (!string.IsNullOrEmpty(searchText))
                    query.Where(x => x.Name.Contains(searchText)).OrderBy(x => x.Name);

                switch (sortBy)
                {
                    case SubsSortBy.Relevance:
                        break; // let db do its thing
                    case SubsSortBy.Subscribers:
                        query.OrderByDescending(x => x.NumberOfSubscribers);
                        break;
                }

                return conn.Select(query);
            });
        }

        public List<Sub> GetDefaultSubs()
        {
            return _conn.Perform(conn =>
            {
                return conn.Select<Sub>(x => x.IsDefault);
            });
        }

        public List<Sub> GetSubscribedSubsForUser(string userName)
        {
            return _conn.Perform(conn =>
            {
                return
                    conn.Select(
                        conn.From<Sub>()
                            .LeftJoin<SubScription>((scription, sub) => scription.Name == sub.SubName)
                            .Where<SubScription>(x => x.UserName == userName));
            });
        }

        public bool IsUserSubscribedToSub(string userName, string subName)
        {
            return _conn.Perform(conn =>
            {
                return conn.Count(
                        conn.From<Sub>()
                            .LeftJoin<SubScription>((scription, sub) => scription.Name == sub.SubName)
                            .Where<SubScription>(x => x.UserName == userName)) > 0;
            });
        }

        public Sub GetRandomSub()
        {
            // todo: optimize
            var allSubs = GetAllSubs();
            if (allSubs.Count == 0)
                return null;
            var rand = new Random();
            return allSubs[rand.Next(allSubs.Count)];
        }

        public void InsertSub(Sub sub)
        {
            _conn.Perform(conn => conn.Insert(sub));
        }

        public void UpdateSub(Sub sub)
        {
            _conn.Perform(conn => conn.Update(sub));
        }

        public void DeleteSub(Guid subId)
        {
            // todo: soft delete
            _conn.Perform(conn => conn.DeleteById<Sub>(subId));
        }

        public void SubscribeToSub(string userName, string subName)
        {
            _conn.Perform(conn =>
            {
                if (conn.Count(conn.From<SubScription>().Where(x => x.UserName == userName && x.SubName == subName)) == 0)
                {
                    conn.Insert(new SubScription
                    {
                        Id = GuidUtil.NewSequentialId(),
                        UserName = userName,
                        SubName = subName
                    });
                }
            });
        }

        public void UnSubscribeToSub(string userName, string subName)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<SubScription>(x => x.UserName.ToLower() == userName.ToLower() && x.SubName.ToLower() == subName.ToLower());
            });
        }

        public Sub GetSubByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            return _conn.Perform(conn =>
            {
                return conn.Single<Sub>(sub => sub.Name.ToLower() == name.ToLower());
            });
        }

        public List<Sub> GetSubByNames(List<string> names)
        {
            if (names == null || names.Count == 0)
                return new List<Sub>();

            return _conn.Perform(conn =>
            {
                return conn.Select(conn.From<Sub>().Where(x => names.Contains(x.Name)));
            });
        }

        public bool CanUserModerateSub(string userName, string subName)
        {
            return _conn.Perform(conn =>
            {
                return conn.Count(conn.From<SubAdmin>().Where(x => x.UserName == userName && x.SubName == subName)) > 0;
            });
        }

        public List<string> GetAllModsForSub(string subName)
        {
            return _conn.Perform(conn =>
            {
                return conn.Select(conn.From<SubAdmin>().Where(x => x.SubName == subName).Select(x => x.UserName))
                    .Select(x => x.UserName).ToList();
            });
        }

        public void AddModToSub(string userName, string subName, string addedBy = null)
        {
            _conn.Perform(conn =>
            {
                if (conn.Count<SubAdmin>(x => x.UserName == userName && x.SubName == subName) > 0)
                    return;

                if (string.IsNullOrEmpty(addedBy))
                    addedBy = "[system]";

                conn.Insert(new SubAdmin
                {
                    Id = GuidUtil.NewSequentialId(),
                    UserName = userName,
                    SubName = subName,
                    AddedOn = Common.CurrentTime(),
                    AddedBy = addedBy
                });
            });
        }

        public void RemoveModFromSub(string userName, string subName)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<SubAdmin>(x => x.UserName == userName && x.SubName == subName);
            });
        }

        public void UpdateNumberOfSubscribers(string subName, out ulong totalNumber)
        {
            ulong temp = 0;
            _conn.Perform(conn =>
            {
                temp = (ulong)conn.Count<SubScription>(x => x.SubName.ToLower() == subName.ToLower());
                conn.Update<Sub>(new { NumberOfSubscribers = temp }, x => x.Name.ToLower() == subName.ToLower());
            });
            totalNumber = temp;
        }
    }
}
