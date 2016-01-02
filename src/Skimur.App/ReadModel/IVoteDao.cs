using System;
using System.Collections.Generic;

namespace Skimur.App.ReadModel
{
    public interface IVoteDao
    {
        VoteType? GetVoteForUserOnPost(Guid userId, Guid postId);

        Dictionary<Guid, VoteType> GetVotesOnPostsByUser(Guid userId, List<Guid> posts);

        VoteType? GetVoteForUserOnComment(Guid userId, Guid commentId);

        Dictionary<Guid, VoteType> GetVotesOnCommentsByUser(Guid userId, List<Guid> comments);
    }
}
