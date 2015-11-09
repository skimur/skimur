using System;
using Membership.Services;
using Skimur.Logging;
using Skimur.Messaging.Handling;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker.Commands
{
    public class PostModerationHandler : 
        ICommandHandlerResponse<ApprovePost, ApprovePostResponse>,
        ICommandHandlerResponse<RemovePost, RemovePostResponse>
    {
        private readonly IMembershipService _membershipService;
        private readonly ILogger<PostModerationHandler> _logger;
        private readonly IPostService _postService;
        private readonly IPermissionService _permissionService;

        public PostModerationHandler(IMembershipService membershipService, 
            ILogger<PostModerationHandler> logger,
            IPostService postService,
            IPermissionService permissionService)
        {
            _membershipService = membershipService;
            _logger = logger;
            _postService = postService;
            _permissionService = permissionService;
        }

        public ApprovePostResponse Handle(ApprovePost command)
        {
            var response = new ApprovePostResponse();

            try
            {
                var user = _membershipService.GetUserById(command.ApprovedBy);

                if (user == null)
                {
                    response.Error = "Unknown user.";
                    return response;
                }

                var post = _postService.GetPostById(command.PostId);

                if (post == null)
                {
                    response.Error = "Unknown post.";
                    return response;
                }

                // TODO: support a narrower set of permissions
                if (!_permissionService.CanUserManageSubPosts(user, post.SubId))
                {
                    response.Error = "Not allowed.";
                    return response;
                }
                
                _postService.ApprovePost(post.Id, user.Id);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occured approving a post.", ex);
                response.Error = "An unknown exception occured.";
            }

            return response;
        }

        public RemovePostResponse Handle(RemovePost command)
        {
            var response = new RemovePostResponse();

            try
            {
                var user = _membershipService.GetUserById(command.RemovedBy);

                if (user == null)
                {
                    response.Error = "Unknown user.";
                    return response;
                }

                var post = _postService.GetPostById(command.PostId);

                if (post == null)
                {
                    response.Error = "Unknown post.";
                    return response;
                }

                // TODO: support a narrower set of permissions
                if (!_permissionService.CanUserManageSubPosts(user, post.SubId))
                {
                    response.Error = "Not allowed.";
                    return response;
                }

                _postService.RemovePost(post.Id, user.Id);
            }
            catch (Exception ex)
            {
                _logger.Error("An error occured removing a post.", ex);
                response.Error = "An unknown exception occured.";
            }

            return response;
        }
    }
}
