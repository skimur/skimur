using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure.Logging;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;
using Infrastructure.Settings;
using Infrastructure.Utils;
using Membership.Services;
using Skimur;
using Skimur.Markdown;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker.Commands
{
    public class PostHandler :
        ICommandHandlerResponse<CreatePost, CreatePostResponse>,
        ICommandHandlerResponse<EditPostContent, EditPostContentResponse>,
        ICommandHandlerResponse<DeletePost, DeletePostResponse>,
        ICommandHandler<TogglePostNsfw>,
        ICommandHandlerResponse<ToggleSticky, ToggleStickyResponse>
    {
        private readonly IMarkdownCompiler _markdownCompiler;
        private readonly ILogger<PostHandler> _logger;
        private readonly IMembershipService _membershipService;
        private readonly IPostService _postService;
        private readonly ISubService _subService;
        private readonly ISubUserBanService _subUserBanService;
        private readonly ICommandBus _commandBus;
        private readonly IPermissionService _permissionService;
        private readonly ISettingsProvider<SubSettings> _subSettings;

        public PostHandler(IMarkdownCompiler markdownCompiler,
            ILogger<PostHandler> logger,
            IMembershipService membershipService,
            IPostService postService,
            ISubService subService,
            ISubUserBanService subUserBanService,
            ICommandBus commandBus,
            IPermissionService permissionService,
            ISettingsProvider<SubSettings> subSettings)
        {
            _markdownCompiler = markdownCompiler;
            _logger = logger;
            _membershipService = membershipService;
            _postService = postService;
            _subService = subService;
            _subUserBanService = subUserBanService;
            _commandBus = commandBus;
            _permissionService = permissionService;
            _subSettings = subSettings;
        }

        public CreatePostResponse Handle(CreatePost command)
        {
            var response = new CreatePostResponse();

            try
            {
                var user = _membershipService.GetUserById(command.CreatedByUserId);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                if (string.IsNullOrEmpty(command.SubName))
                {
                    response.Error = "The sub name is required.";
                    return response;
                }

                var sub = _subService.GetSubByName(command.SubName);

                if (sub == null)
                {
                    response.Error = "That sub doesn't exist.";
                    return response;
                }

                if (!user.IsAdmin)
                {
                    // make sure the user isn't banned from this sub
                    if (_subUserBanService.IsUserBannedFromSub(sub.Id, user.Id))
                    {
                        response.Error = "You are currently banned from this sub.";
                        return response;
                    }
                }

                // TODO: does user look like spam? 

                // todo: make sure the post type is allowed

                if (string.IsNullOrEmpty(command.Title))
                {
                    response.Error = "The title is required.";
                    return response;
                }

                // remove extrenous white space, and trim whitespace from the edges
                command.Title = Regex.Replace(command.Title, @"\s+", " ").Trim();

                if (command.Title.Length > 300)
                {
                    response.Error = "The title is too long.";
                    return response;
                }

                string domain = null;

                if (command.PostType == PostType.Link)
                {
                    if (string.IsNullOrEmpty(command.Url))
                    {
                        response.Error = "You must provide a url.";
                        return response;
                    }

                    // todo: improve url validation
                    string scheme;
                    if (!UrlParser.TryParseUrl(command.Url, out domain, out scheme))
                    {
                        response.Error = "The url appears to be invalid.";
                        return response;
                    }

                    switch (scheme)
                    {
                        case "http":
                        case "https":
                            break;
                        default:
                            response.Error = "The scheme is invalid for the url.";
                            return response;
                    }

                    // todo: make sure the domain isn't banned

                    // todo: make sure the url wasn't already submitted
                    
                }
                else if (command.PostType == PostType.Text)
                {
                    if (!user.IsAdmin)
                    {
                        if (!string.IsNullOrEmpty(command.Content) && command.Content.Length > 40000)
                        {
                            response.Error = "The post content is too long (maximum 40000 characters).";
                            return response;
                        }
                    }
                }
                else
                {
                    throw new Exception("unknown post type " + command.PostType);
                }

                bool isNsfw;

                if (sub.Nsfw)
                    // NSFW by default
                    isNsfw = true;
                else
                    // Let's see if the user marked this as NSFW.
                    isNsfw = Common.IsNsfw(command.Title);
                
                var post = new Post
                {
                    Id = GuidUtil.NewSequentialId(),
                    DateCreated = command.OverrideDateCreated.HasValue ? command.OverrideDateCreated.Value : Common.CurrentTime(),
                    LastEditDate = null,
                    SubId = sub.Id,
                    UserId = user.Id,
                    UserIp = command.IpAddress,
                    PostType = command.PostType,
                    Title = command.Title,
                    SendReplies = command.NotifyReplies,
                    Mirrored = command.Mirror,
                    Nsfw = isNsfw,
                    InAll = sub.InAll
                };

                if (post.PostType == PostType.Link)
                {
                    post.Url = command.Url;
                    post.Domain = domain;
                }
                else
                {
                    post.Content = command.Content;
                    post.ContentFormatted = _markdownCompiler.Compile(post.Content);
                }

                _postService.InsertPost(post);
                _commandBus.Send(new CastVoteForPost { DateCasted = post.DateCreated, IpAddress = command.IpAddress, PostId = post.Id, UserId = user.Id, VoteType = VoteType.Up });

                response.Title = command.Title;
                response.PostId = post.Id;
            }
            catch (Exception ex)
            {
                // todo: log
                response.Error = ex.Message;
            }

            return response;
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

                    // let's remove the single vote that the author may have attributed to this post.
                    // this will prevent people from creating/deleting post for a single kudo, over and over.
                    _commandBus.Send(new CastVoteForPost
                    {
                        VoteType = null, // unvote the comment!
                        PostId = post.Id,
                        DateCasted = Common.CurrentTime(),
                        IpAddress = string.Empty, // TODO,
                        UserId = post.UserId
                    });

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

        public void Handle(TogglePostNsfw command)
        {
            var user = _membershipService.GetUserById(command.UserId);
            if (user == null) return;

            var post = _postService.GetPostById(command.PostId);
            if (post == null) return;

            if (post.Nsfw == command.IsNsfw) return; // nothing to change

            if (!_permissionService.CanUserManageSubPosts(user, post.SubId)) return;

            post.Nsfw = command.IsNsfw;

            _postService.UpdatePost(post);
        }

        public ToggleStickyResponse Handle(ToggleSticky command)
        {
            var response = new ToggleStickyResponse();

            try
            {
                var post = _postService.GetPostById(command.PostId);
                if (post == null)
                {
                    response.Error = "Invalid post.";
                    return response;
                }

                var user = _membershipService.GetUserById(command.UserId);
                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                if (!_permissionService.CanUserManageSubPosts(user, post.SubId))
                {
                    response.Error = "You are not authorized to manage stickies for this sub.";
                    return response;
                }

                if (command.Sticky == post.Sticky)
                    return response; // already done, no error

                if (command.Sticky)
                {
                    // we are trying to sticky something, let's see if we reached our limit.
                    // we don't need to check this limit of we are UN-stickying a post.
                    var currentStickied = _postService.GetPosts(new List<Guid> {post.SubId}, sticky: true);
                    if (currentStickied.Count >= _subSettings.Settings.MaximumNumberOfStickyPosts)
                    {
                        response.Error = string.Format("You are only allowed {0} stickied posts.",
                            _subSettings.Settings.MaximumNumberOfStickyPosts);
                        return response;
                    }
                }

                _postService.SetStickyForPost(post.Id, command.Sticky);
            }
            catch (Exception ex)
            {
                _logger.Error("An unknown error occured attempting to sticky a post.", ex);
                response.Error = "An unknown error occured.";
            }

            return response;
        }
    }
}
