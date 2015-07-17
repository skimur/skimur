using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Utils;
using ServiceStack.OrmLite;
using Skimur;

namespace Subs.Services
{
    public class VoteService : IVoteService
    {
        private readonly IDbConnectionProvider _conn;

        public VoteService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        public void VoteForPost(string postSlug, string userName, string ipAddress, VoteType voteType, DateTime dateCasted)
        {
            _conn.Perform(conn =>
            {
                var vote = conn.Single<Vote>(x => x.UserName.ToLower() == userName.ToLower() && x.PostSlug.ToLower() == postSlug.ToLower());

                if (vote == null)
                {
                    conn.Insert(new Vote
                    {
                        Id = GuidUtil.NewSequentialId(),
                        DateCreated = Common.CurrentTime(),
                        UserName = userName,
                        PostSlug = postSlug,
                        VoteType = voteType,
                        DateCasted = dateCasted,
                        IpAddress = ipAddress
                    });
                }
                else
                {
                    if (vote.VoteType != voteType)
                        conn.Update<Vote>(new { Type = (int)voteType }, x => x.Id == vote.Id);
                }
            });
        }

        public void UnVotePost(string postSlug, string userName)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<Vote>(x => x.PostSlug.ToLower() == postSlug.ToLower() && x.UserName.ToLower() == userName.ToLower());
            });
        }

        public VoteType? GetVoteForUserOnPost(string userName, string postSlug)
        {
            if (string.IsNullOrEmpty(postSlug)) return null;

            return _conn.Perform(conn =>
            {
                var vote = conn.Single<Vote>(x => x.UserName.ToLower() == userName.ToLower() && x.PostSlug.ToLower() == postSlug.ToLower());

                if (vote == null)
                    return (VoteType?)null;

                return vote.VoteType;
            });
        }

        public void GetTotalVotesForPost(string postSlug, out int upVotes, out int downVotes)
        {
            upVotes = 0;
            downVotes = 0;
            int tempUpVotes = 0;
            int tempDownVotes = 0;
            _conn.Perform(conn =>
            {
                tempUpVotes = (int)conn.Count(conn.From<Vote>()
                            .Where(x => x.PostSlug.ToLower() == postSlug.ToLower() && x.Type == (int)VoteType.Up));
                tempDownVotes = (int)conn.Count(conn.From<Vote>()
                            .Where(x => x.PostSlug.ToLower() == postSlug.ToLower() && x.Type == (int)VoteType.Down));
            });
            upVotes = tempUpVotes;
            downVotes = tempDownVotes;
        }
    }
}
