using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Logging;
using Infrastructure.Messaging.Handling;
using Membership.Services;
using Skimur.Markdown;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker.Commands
{
    public class PostHandler : 
        ICommandHandlerResponse<EditPostContent, EditPostContentResponse>,
        ICommandHandlerResponse<DeletePost, DeletePostResponse>
    {
        private readonly IMarkdownCompiler _markdownCompiler;
        private readonly ILogger<PostHandler> _logger;
        private readonly IMembershipService _membershipService;
        private readonly IPostService _postService;

        public PostHandler(IMarkdownCompiler markdownCompiler, 
            ILogger<PostHandler> logger,
            IMembershipService membershipService,
            IPostService postService)
        {
            _markdownCompiler = markdownCompiler;
            _logger = logger;
            _membershipService = membershipService;
            _postService = postService;
        }

        public EditPostContentResponse Handle(EditPostContent command)
        {
            var response = new EditPostContentResponse();

            try
            {
                var user = _membershipService.GetUserById(command.EditedBy);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var post = _postService.GetPostById(command.PostId);

                if (post == null)
                {
                    response.Error = "Invalid post.";
                    return response;
                }

                if (post.UserId != user.Id)
                {
                    response.Error = "You are not allowed to edit another user's post.";
                    return response;
                }

                if (post.PostType != PostType.Text)
                {
                    response.Error = "You can only edit text posts.";
                    return response;
                }

                if (!user.IsAdmin)
                {
                    if (!string.IsNullOrEmpty(command.Content) && command.Content.Length > 40000)
                    {
                        response.Error = "The post content is too long (maximum 40000 characters).";
                        return response;
                    }
                }

                post.Content = command.Content;
                post.ContentFormatted = _markdownCompiler.Compile(post.Content);

                _postService.UpdatePost(post);

                response.Content = post.Content;
                response.ContentFormatted = post.ContentFormatted;

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Error editing post.", ex);
                response.Error = "An unknown error occured.";
                return response;
            }
        }

        public DeletePostResponse Handle(DeletePost command)
        {
            var response = new DeletePostResponse();

            try
            {
                var user = _membershipService.GetUserById(command.DeleteBy);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var post = _postService.GetPostById(command.PostId);

                if (post == null)
                {
                    response.Error = "Invalid post.";
                    return response;
                }

                if (post.UserId == user.Id || user.IsAdmin)
                {
                    post.Deleted = true;   
                    _postService.UpdatePost(post);
                }
                else
                {
                    response.Error = "You cannot delete this post.";
                    return response;
                }
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error("Error editing post.", ex);
                response.Error = "An unknown error occured.";
                return response;
            }
        }
    }
}
