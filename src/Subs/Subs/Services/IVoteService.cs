using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.Services
{
    public interface IVoteService
    {
        void VoteForPost(string postSlug, string userName, string ipAddress, VoteType voteType, DateTime dateCasted);

        void UnVotePost(string postSlug, string userName);

        VoteType? GetVoteForUserOnPost(string userName, string postSlug);
        
        void GetTotalVotesForPost(string postSlug, out int upVotes, out int downVotes);

        void VoteForComment(Guid commentId, string userName, string ipAddress, VoteType voteType, DateTime dateCasted);

        void UnVoteComment(Guid commentId, string userName);

        VoteType? GetVoteForUserOnComment(string userName, Guid commentId);

        Dictionary<Guid, VoteType> GetVotesOnCommentsByUser(string userName, List<Guid> comments);

        void GetTotalVotesForComment(Guid commentId, out int upVotes, out int downVotes);
    }
}
