using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel
{
    public interface IVoteDao
    {
        VoteType? GetVoteForUserOnPost(Guid userId, Guid postId);

        Dictionary<Guid, VoteType> GetVotesOnPostsByUser(Guid userId, List<Guid> posts);

        VoteType? GetVoteForUserOnComment(Guid userId, Guid commentId);

        Dictionary<Guid, VoteType> GetVotesOnCommentsByUser(Guid userId, List<Guid> comments);
    }
}
