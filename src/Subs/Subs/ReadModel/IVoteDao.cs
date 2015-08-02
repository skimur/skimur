using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel
{
    public interface IVoteDao
    {
        VoteType? GetVoteForUserOnPost(string userName, string postSlug);

        VoteType? GetVoteForUserOnComment(string userName, Guid commentId);

        Dictionary<Guid, VoteType> GetVotesOnCommentsByUser(string userName, List<Guid> comments);
    }
}
