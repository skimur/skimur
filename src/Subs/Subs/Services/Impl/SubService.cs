using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Utils;
using ServiceStack.OrmLite;
using Skimur;
using Subs.ReadModel;

namespace Subs.Services.Impl
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

        public SeekedList<Guid> GetAllSubs(string searchText = null, SubsSortBy sortBy = SubsSortBy.Relevance, int? skip = null, int? take = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Sub>();
                if (!string.IsNullOrEmpty(searchText))
                    query.Where(x => x.Name.Contains(searchText)).OrderBy(x => x.Name);

                var totalCount = conn.Count(query);

                query.Skip(skip).Take(take);

                switch (sortBy)
                {
                    case SubsSortBy.Relevance:
                        break; // let db do its thing
                    case SubsSortBy.Subscribers:
                        query.OrderByDescending(x => x.NumberOfSubscribers);
                        break;
                }

                query.SelectExpression = "SELECT \"id\"";

                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }

        public List<Guid> GetDefaultSubs()
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Sub>().Where(x => x.IsDefault);
                query.SelectExpression = "SELECT \"id\"";
                return conn.Select(query).Select(x => x.Id).ToList();
            });
        }

        public List<Guid> GetSubscribedSubsForUser(Guid userId)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Sub>()
                    .LeftJoin<SubScription>((sub, scription) => sub.Id == scription.SubId)
                    .Where<SubScription>(x => x.UserId == userId);

                query.SelectExpression = "";

                return conn.Select(query).Select(x => x.Id).ToList();
            });
        }

        public bool IsUserSubscribedToSub(Guid userId, Guid subId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Count(
                        conn.From<Sub>()
                             .LeftJoin<SubScription>((sub, scription) => sub.Id == scription.SubId)
                            .Where<SubScription>(x => x.UserId == userId)) > 0;
            });
        }

        public Guid? GetRandomSub()
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

        public void SubscribeToSub(Guid userId, Guid subId)
        {
            _conn.Perform(conn =>
            {
                if (conn.Count(conn.From<SubScription>().Where(x => x.UserId == userId && x.SubId == subId)) == 0)
                {
                    conn.Insert(new SubScription
                    {
                        Id = GuidUtil.NewSequentialId(),
                        UserId = userId,
                        SubId = subId
                    });
                }
            });
        }

        public void UnSubscribeToSub(Guid userId, Guid subId)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<SubScription>(x => x.UserId == userId && x.SubId == subId);
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

        public List<Sub> GetSubsByIds(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<Sub>();

            return _conn.Perform(conn =>
            {
                return conn.Select(conn.From<Sub>().Where(x => ids.Contains(x.Id)));
            });
        }

        public Sub GetSubById(Guid id)
        {
            return _conn.Perform(conn => conn.SingleById<Sub>(id));
        }

        public bool CanUserModerateSub(Guid userId, Guid subId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Count(conn.From<SubAdmin>().Where(x => x.UserId == userId && x.SubId == subId)) > 0;
            });
        }

        public List<Guid> GetAllModsForSub(Guid subId)
        {
            return _conn.Perform(conn =>
            {
                return conn.Select(conn.From<SubAdmin>().Where(x => x.SubId == subId).Select(x => x.UserId))
                    .Select(x => x.UserId).ToList();
            });
        }

        public void AddModToSub(Guid userId, Guid subId, Guid? addedBy = null)
        {
            _conn.Perform(conn =>
            {
                if (conn.Count<SubAdmin>(x => x.UserId == userId && x.SubId == subId) > 0)
                    return;
                
                conn.Insert(new SubAdmin
                {
                    Id = GuidUtil.NewSequentialId(),
                    UserId = userId,
                    SubId = subId,
                    AddedOn = Common.CurrentTime(),
                    AddedBy = addedBy
                });
            });
        }

        public void RemoveModFromSub(Guid userId, Guid subId)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<SubAdmin>(x => x.UserId == userId && x.SubId == subId);
            });
        }

        public void UpdateNumberOfSubscribers(Guid subId, out ulong totalNumber)
        {
            ulong temp = 0;
            _conn.Perform(conn =>
            {
                temp = (ulong)conn.Count<SubScription>(x => x.SubId == subId);
                conn.Update<Sub>(new { NumberOfSubscribers = temp }, x => x.Id == subId);
            });
            totalNumber = temp;
        }
    }
}
