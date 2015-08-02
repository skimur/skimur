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

            if (string.IsNullOrEmpty(userName)) return null;

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

        public void VoteForComment(Guid commentId, string userName, string ipAddress, VoteType voteType, DateTime dateCasted)
        {
            _conn.Perform(conn =>
            {
                var vote = conn.Single<Vote>(x => x.UserName.ToLower() == userName.ToLower() && x.CommentId == commentId);

                if (vote == null)
                {
                    conn.Insert(new Vote
                    {
                        Id = GuidUtil.NewSequentialId(),
                        DateCreated = Common.CurrentTime(),
                        UserName = userName,
                        CommentId = commentId,
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

        public void UnVoteComment(Guid commentId, string userName)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<Vote>(x => x.CommentId == commentId && x.UserName.ToLower() == userName.ToLower());
            });
        }

        public VoteType? GetVoteForUserOnComment(string userName, Guid commentId)
        {
            if (commentId == Guid.Empty) return null;

            if (string.IsNullOrEmpty(userName)) return null;

            return _conn.Perform(conn =>
            {
                var vote = conn.Single<Vote>(x => x.UserName.ToLower() == userName.ToLower() && x.CommentId == commentId);

                if (vote == null)
                    return (VoteType?)null;

                return vote.VoteType;
            });
        }

        public Dictionary<Guid, VoteType> GetVotesOnCommentsByUser(string userName, List<Guid> comments)
        {
            if (string.IsNullOrEmpty(userName)) return new Dictionary<Guid, VoteType>();

            if (comments == null || comments.Count == 0)
                return new Dictionary<Guid, VoteType>();

            return _conn.Perform(conn =>
            {
                var query = conn.From<Vote>().Where(x => x.UserName.ToLower() == userName.ToLower() && comments.Contains((Guid)x.CommentId));
                query.SelectExpression = "SELECT \"comment_id\", \"type\"";
                return conn.Select(query).ToDictionary(x => x.CommentId.Value, x => x.VoteType);
            });
        }
        
        public void GetTotalVotesForComment(Guid commentId, out int upVotes, out int downVotes)
        {
            int tempUpVotes = 0;
            int tempDownVotes = 0;
            _conn.Perform(conn =>
            {
                tempUpVotes = (int)conn.Count(conn.From<Vote>()
                            .Where(x => x.CommentId == commentId && x.Type == (int)VoteType.Up));
                tempDownVotes = (int)conn.Count(conn.From<Vote>()
                            .Where(x => x.CommentId == commentId && x.Type == (int)VoteType.Down));
            });
            upVotes = tempUpVotes;
            downVotes = tempDownVotes;
        }
    }
}
