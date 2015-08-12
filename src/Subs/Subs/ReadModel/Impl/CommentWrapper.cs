using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Membership;

namespace Subs.ReadModel.Impl
{
    public class CommentWrapper : ICommentWrapper
    {
        private readonly ICommentDao _commentDao;
        private readonly IMembershipService _membershipService;
        private readonly ISubDao _subDao;
        private readonly IPostDao _postDao;
        private readonly IPermissionDao _permissionDao;
        private readonly IVoteDao _voteDao;

        public CommentWrapper(ICommentDao commentDao, IMembershipService membershipService, ISubDao subDao, IPostDao postDao, IPermissionDao permissionDao, IVoteDao voteDao)
        {
            _commentDao = commentDao;
            _membershipService = membershipService;
            _subDao = subDao;
            _postDao = postDao;
            _permissionDao = permissionDao;
            _voteDao = voteDao;
        }

        public List<CommentWrapped> Wrap(List<Guid> commentIds, User currentUser = null)
        {
            var result = new List<CommentWrapped>();
            foreach (var commentId in commentIds)
            {
                var comment = _commentDao.GetCommentById(commentId);
                if (comment != null)
                    result.Add(new CommentWrapped(comment));
            }

            var authors = _membershipService.GetUsersByIds(result.Select(x => x.Comment.AuthorUserId).Distinct().ToList()).ToDictionary(x => x.Id, x => x);
            var subs = _subDao.GetSubsByIds(result.Select(x => x.Comment.SubId).Distinct().ToList()).ToDictionary(x => x.Id, x => x);
            var posts = result.Select(x => x.Comment.PostId).Distinct().Select(x => _postDao.GetPostById(x)).Where(x => x != null).ToDictionary(x => x.Id, x => x);

            var userCanModInSubs = new List<Guid>();

            if (currentUser != null)
                foreach (var sub in subs.Values)
                    // TODO: Check for a specific permission "ban".
                    if (_permissionDao.CanUserModerateSub(currentUser.Id, sub.Id))
                        userCanModInSubs.Add(sub.Id);

            var likes = currentUser != null ? _voteDao.GetVotesOnCommentsByUser(currentUser.Id, commentIds) : new Dictionary<Guid, VoteType>();

            foreach (var item in result)
            {
                item.Author = authors.ContainsKey(item.Comment.AuthorUserId) ? authors[item.Comment.AuthorUserId] : null;
                item.CurrentUserVote = likes.ContainsKey(item.Comment.Id) ? likes[item.Comment.Id] : (VoteType?)null;
                item.Sub = subs.ContainsKey(item.Comment.SubId) ? subs[item.Comment.SubId] : null;
                item.Score = item.Comment.VoteUpCount - item.Comment.VoteDownCount;
                item.Post = posts.ContainsKey(item.Comment.PostId) ? posts[item.Comment.PostId] : null;

                var userCanMod = item.Sub != null && userCanModInSubs.Contains(item.Sub.Id);
                
                if ((item.Author != null && currentUser != null) && currentUser.Id == item.Author.Id)
                    item.CurrentUserIsAuthor = true;

                item.CanDelete = userCanMod || item.CurrentUserIsAuthor;
                item.CanEdit = item.CurrentUserIsAuthor;
            }

            return result;
        }

        public CommentWrapped Wrap(Guid commentId, User currentUser = null)
        {
            return Wrap(new List<Guid> {commentId}, currentUser)[0];
        }
    }
}
