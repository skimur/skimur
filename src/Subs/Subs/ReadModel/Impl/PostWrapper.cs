using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Membership;

namespace Subs.ReadModel.Impl
{
    public class PostWrapper : IPostWrapper
    {
        private readonly IPostDao _postDao;
        private readonly IMembershipService _membershipService;
        private readonly ISubDao _subDao;
        private readonly IVoteDao _voteDao;

        public PostWrapper(IPostDao postDao, IMembershipService membershipService, ISubDao subDao, IVoteDao voteDao)
        {
            _postDao = postDao;
            _membershipService = membershipService;
            _subDao = subDao;
            _voteDao = voteDao;
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

            foreach (var item in posts)
            {
                item.Author = authors.ContainsKey(item.Post.UserId) ? authors[item.Post.UserId] : null;
                item.Sub = subs.ContainsKey(item.Post.SubId) ? subs[item.Post.SubId] : null;
                if (currentUser != null)
                    item.CurrentUserVote = likes.ContainsKey(item.Post.Id) ? likes[item.Post.Id] : (VoteType?)null;
            }

            return posts;
        }

        public PostWrapped Wrap(Guid postId, User currentUser = null)
        {
            return Wrap(new List<Guid> {postId}, currentUser)[0];
        }
    }
}
