using System;
using System.Collections.Generic;
using System.Linq;
using Membership;
using Membership.Services;

namespace Subs.ReadModel.Impl
{
    public class PostWrapper : IPostWrapper
    {
        private readonly IPostDao _postDao;
        private readonly IMembershipService _membershipService;
        private readonly ISubDao _subDao;
        private readonly IVoteDao _voteDao;
        private readonly IPermissionDao _permissionDao;

        public PostWrapper(IPostDao postDao, 
            IMembershipService membershipService, 
            ISubDao subDao, 
            IVoteDao voteDao,
            IPermissionDao permissionDao)
        {
            _postDao = postDao;
            _membershipService = membershipService;
            _subDao = subDao;
            _voteDao = voteDao;
            _permissionDao = permissionDao;
        }

        public List<PostWrapped> Wrap(List<Guid> postIds, User currentUser = null)
        {
            var posts = new List<PostWrapped>();

            foreach (var postId in postIds)
            {
                var post = _postDao.GetPostById(postId);
                if (post != null)
                    posts.Add(new PostWrapped(post));
            }

            var authors = _membershipService.GetUsersByIds(posts.Select(x => x.Post.UserId).Distinct().ToList()).ToDictionary(x => x.Id, x => x);
            var subs = _subDao.GetSubsByIds(posts.Select(x => x.Post.SubId).Distinct().ToList()).ToDictionary(x => x.Id, x => x);
            var likes = currentUser != null ? _voteDao.GetVotesOnPostsByUser(currentUser.Id, postIds) : new Dictionary<Guid, VoteType>();
            var canManagePosts = currentUser != null
                // TODO: add support for narrowed permissions for managing posts
                ? subs.Values.Where(x => _permissionDao.CanUserModerateSub(currentUser, x.Id))
                    .Select(x => x.Id)
                    .ToList()
                : new List<Guid>();
                
            foreach (var item in posts)
            {
                item.Author = authors.ContainsKey(item.Post.UserId) ? authors[item.Post.UserId] : null;
                item.Sub = subs.ContainsKey(item.Post.SubId) ? subs[item.Post.SubId] : null;
                if (currentUser != null)
                    item.CurrentUserVote = likes.ContainsKey(item.Post.Id) ? likes[item.Post.Id] : (VoteType?)null;
                if (canManagePosts.Contains(item.Post.SubId))
                {
                    // this user can approve/disapprove of a post, mark it NSFW, etc
                    item.CanManagePost = true;
                    item.Verdict = item.Post.PostVerdict;
                }
            }

            return posts;
        }

        public PostWrapped Wrap(Guid postId, User currentUser = null)
        {
            return Wrap(new List<Guid> {postId}, currentUser)[0];
        }
    }
}
