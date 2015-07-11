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
    }
}
