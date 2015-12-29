using System.Linq;
using Skimur.App.Commands;
using Skimur.App.Services;
using Skimur.Messaging.Handling;

namespace Skimur.App.Handlers.Commands
{
    public class ReportHandler : 
        ICommandHandler<ReportComment>,
        ICommandHandler<ReportPost>,
        ICommandHandler<ConfigureReportIgnoring>,
        ICommandHandler<ClearReports>
    {
        private readonly IMembershipService _membershipService;
        private readonly ICommentService _commentService;
        private readonly IPostService _postService;
        private readonly IReportService _reportService;
        private readonly ISubUserBanService _subUserBanService;
        private readonly IPermissionService _permissionService;

        public ReportHandler(IMembershipService membershipService, 
            ICommentService commentService,
            IPostService postService,
            IReportService reportService,
            ISubUserBanService subUserBanService,
            IPermissionService permissionService)
        {
            _membershipService = membershipService;
            _commentService = commentService;
            _postService = postService;
            _reportService = reportService;
            _subUserBanService = subUserBanService;
            _permissionService = permissionService;
        }

        public void Handle(ReportComment command)
        {
            if (string.IsNullOrEmpty(command.Reason)) return;
            if (command.Reason.Length > 200)
                command.Reason = command.Reason.Substring(0, 200);

            var user = _membershipService.GetUserById(command.ReportBy);
            if (user == null) return;

            var comment = _commentService.GetCommentById(command.CommentId);
            if(comment == null) return;

            // did a mod/admin configure this comment to ignore any reports?
            if (comment.IgnoreReports) return;

            // the user can't report things in a sub they are banned from
            if (_subUserBanService.IsUserBannedFromSub(comment.Id, user.Id)) return;

            // make sure the user hasn't already report the comment
            var currentReports = _reportService.GetReportsForComment(comment.Id);
            if (currentReports.Any(x => x.ReportedBy == user.Id)) return;

            _reportService.ReportComment(comment.Id, user.Id, command.Reason);

            _commentService.UpdateNumberOfReportsForComment(comment.Id, currentReports.Count + 1);
        }

        public void Handle(ReportPost command)
        {
            if (string.IsNullOrEmpty(command.Reason)) return;
            if (command.Reason.Length > 200)
                command.Reason = command.Reason.Substring(0, 200);

            var user = _membershipService.GetUserById(command.ReportBy);
            if (user == null) return;

            var post = _postService.GetPostById(command.PostId);
            if (post == null) return;

            // did a mod/admin configure this post to ignore any reports?
            if (post.IgnoreReports) return;

            // the user can't report things in a sub they are banned from
            if (_subUserBanService.IsUserBannedFromSub(post.SubId, user.Id)) return;

            // make sure the user hasn't already report the comment
            var currentReports = _reportService.GetReportsForPost(post.Id);
            if (currentReports.Any(x => x.ReportedBy == user.Id)) return;

            _reportService.ReportPost(post.Id, user.Id, command.Reason);

            _postService.UpdateNumberOfReportsForPost(post.Id, currentReports.Count + 1);
        }
        
        public void Handle(ConfigureReportIgnoring command)
        {
            if (!command.PostId.HasValue && !command.CommentId.HasValue) return;

            var user = _membershipService.GetUserById(command.UserId);

            if (user == null) return;

            if (command.PostId.HasValue)
            {
                var post = _postService.GetPostById(command.PostId.Value);
                if (post == null) return;

                if (!_permissionService.CanUserManageSubPosts(user, post.SubId)) return;

                if (post.IgnoreReports == command.IgnoreReports) return;

                _postService.SetIgnoreReportsForPost(post.Id, command.IgnoreReports);

                if (command.IgnoreReports)
                {
                    // delete any existing reports
                    _reportService.RemoveReportsForPost(post.Id);
                    _postService.UpdateNumberOfReportsForPost(post.Id, 0);
                }
            }

            if (command.CommentId.HasValue)
            {
                var comment = _commentService.GetCommentById(command.CommentId.Value);
                if (comment == null) return;

                if (!_permissionService.CanUserManageSubPosts(user, comment.SubId)) return;

                if (comment.IgnoreReports == command.IgnoreReports) return;

                _commentService.SetIgnoreReportsForComment(comment.Id, command.IgnoreReports);

                if (command.IgnoreReports)
                {
                    // delete any existing reports
                    _reportService.RemoveReportsForComment(comment.Id);
                    _commentService.UpdateNumberOfReportsForComment(comment.Id, 0);
                }
            }
        }

        public void Handle(ClearReports command)
        {
            if (!command.PostId.HasValue && !command.CommentId.HasValue)
                return;

            var user = _membershipService.GetUserById(command.UserId);
            if (user == null) return;

            if (command.PostId.HasValue)
            {
                var post = _postService.GetPostById(command.PostId.Value);
                if (post == null) return;
                
                if (_permissionService.CanUserManageSubPosts(user, post.SubId))
                {
                    _reportService.RemoveReportsForPost(post.Id);
                    _postService.UpdateNumberOfReportsForPost(post.Id, 0);
                }
            }

            if (command.CommentId.HasValue)
            {
                var comment = _commentService.GetCommentById(command.CommentId.Value);
                if (comment == null) return;
                
                if (_permissionService.CanUserManageSubPosts(user, comment.SubId))
                {
                    _reportService.RemoveReportsForComment(comment.Id);
                    _commentService.UpdateNumberOfReportsForComment(comment.Id, 0);
                }
            }
        }
    }
}
