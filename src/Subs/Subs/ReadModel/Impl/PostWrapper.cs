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
        private readonly IReportDao _reportDao;

        public PostWrapper(IPostDao postDao, 
            IMembershipService membershipService, 
            ISubDao subDao, 
            IVoteDao voteDao,
            IPermissionDao permissionDao,
            IReportDao reportDao)
        {
            _postDao = postDao;
            _membershipService = membershipService;
            _subDao = subDao;
            _voteDao = voteDao;
            _permissionDao = permissionDao;
            _reportDao = reportDao;
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
                ? subs.Values.Where(x => _permissionDao.CanUserManageSubPosts(currentUser, x.Id))
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
                    item.CanManage = true;
                    item.Verdict = item.Post.PostVerdict;

                    if (item.Post.NumberOfReports > 0)
                    {
                        var reports = _reportDao.GetReportsForPost(item.Post.Id);
                        item.Reports = new List<ReportSummary>();
                        foreach (var report in reports)
                        {
                            var summary = new ReportSummary();
                            if (!authors.ContainsKey(report.ReportedBy))
                                authors.Add(report.ReportedBy, _membershipService.GetUserById(report.ReportedBy));
                            var user = authors[report.ReportedBy];
                            if (user != null)
                                summary.UserName = user.UserName;
                            summary.Reason = report.Reason;
                            item.Reports.Add(summary);
                        }
                    }
                }
                if (currentUser != null)
                    item.CanReport = true;

                // authors can only edit text posts
                if (item.Post.PostType == PostType.Text && currentUser != null && currentUser.Id == item.Post.UserId)
                    item.CanEdit = true;
            }

            return posts;
        }

        public PostWrapped Wrap(Guid postId, User currentUser = null)
        {
            return Wrap(new List<Guid> {postId}, currentUser)[0];
        }
    }
}
