using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface IVoteService
    {
        void VoteForPost(Guid postId, Guid userId, string ipAddress, VoteType voteType, DateTime dateCasted);

        void UnVotePost(Guid postId, Guid userId);

        VoteType? GetVoteForUserOnPost(Guid userId, Guid postId);

        Dictionary<Guid, VoteType> GetVotesOnPostsByUser(Guid userId, List<Guid> posts);

        void GetTotalVotesForPost(Guid postId, out int upVotes, out int downVotes);

        void VoteForComment(Guid commentId, Guid userId, string ipAddress, VoteType voteType, DateTime dateCasted);

        void UnVoteComment(Guid commentId, Guid userId);

        VoteType? GetVoteForUserOnComment(Guid userId, Guid commentId);

        Dictionary<Guid, VoteType> GetVotesOnCommentsByUser(Guid userId, List<Guid> comments);

        void GetTotalVotesForComment(Guid commentId, out int upVotes, out int downVotes);
    }
}
