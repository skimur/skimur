using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Data;
using Infrastructure.Utils;
using ServiceStack.OrmLite;
using Skimur;

namespace Subs.Services.Impl
{
    public class VoteService : IVoteService
    {
        private readonly IDbConnectionProvider _conn;

        public VoteService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        public void VoteForPost(Guid postId, Guid userId, string ipAddress, VoteType voteType, DateTime dateCasted)
        {
            _conn.Perform(conn =>
            {
                var vote = conn.Single<Vote>(x => x.UserId == userId && x.PostId == postId);

                if (vote == null)
                {
                    conn.Insert(new Vote
                    {
                        Id = GuidUtil.NewSequentialId(),
                        DateCreated = Common.CurrentTime(),
                        UserId = userId,
                        PostId = postId,
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

        public void UnVotePost(Guid postId, Guid userId)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<Vote>(x => x.PostId == postId && x.UserId == userId);
            });
        }

        public VoteType? GetVoteForUserOnPost(Guid userId, Guid postId)
        {
            return _conn.Perform(conn =>
            {
                var vote = conn.Single<Vote>(x => x.UserId == userId && x.PostId == postId);

                if (vote == null)
                    return (VoteType?)null;

                return vote.VoteType;
            });
        }

        public Dictionary<Guid, VoteType> GetVotesOnPostsByUser(Guid userId, List<Guid> posts)
        {
            if (posts == null || posts.Count == 0)
                return new Dictionary<Guid, VoteType>();

            return _conn.Perform(conn =>
            {
                var query = conn.From<Vote>().Where(x => x.UserId == userId && posts.Contains((Guid)x.PostId));
                query.SelectExpression = "SELECT \"post_id\", \"type\"";
                return conn.Select(query).ToDictionary(x => x.PostId.Value, x => x.VoteType);
            });
        }

        public void GetTotalVotesForPost(Guid postId, out int upVotes, out int downVotes)
        {
            int tempUpVotes = 0;
            int tempDownVotes = 0;
            _conn.Perform(conn =>
            {
                tempUpVotes = (int)conn.Count(conn.From<Vote>()
                            .Where(x => x.PostId == postId && x.Type == (int)VoteType.Up));
                tempDownVotes = (int)conn.Count(conn.From<Vote>()
                            .Where(x => x.PostId == postId && x.Type == (int)VoteType.Down));
            });
            upVotes = tempUpVotes;
            downVotes = tempDownVotes;
        }

        public void VoteForComment(Guid commentId, Guid userId, string ipAddress, VoteType voteType, DateTime dateCasted)
        {
            _conn.Perform(conn =>
            {
                var vote = conn.Single<Vote>(x => x.UserId == userId && x.CommentId == commentId);

                if (vote == null)
                {
                    conn.Insert(new Vote
                    {
                        Id = GuidUtil.NewSequentialId(),
                        DateCreated = Common.CurrentTime(),
                        UserId = userId,
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

        public void UnVoteComment(Guid commentId, Guid userId)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<Vote>(x => x.CommentId == commentId && x.UserId == userId);
            });
        }

        public VoteType? GetVoteForUserOnComment(Guid userId, Guid commentId)
        {
            return _conn.Perform(conn =>
            {
                var vote = conn.Single<Vote>(x => x.UserId == userId && x.CommentId == commentId);

                if (vote == null)
                    return (VoteType?)null;

                return vote.VoteType;
            });
        }

        public Dictionary<Guid, VoteType> GetVotesOnCommentsByUser(Guid userId, List<Guid> comments)
        {
            if (comments == null || comments.Count == 0)
                return new Dictionary<Guid, VoteType>();

            return _conn.Perform(conn =>
            {
                var query = conn.From<Vote>().Where(x => x.UserId == userId && comments.Contains((Guid)x.CommentId));
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
