using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Membership.Services;
using Skimur;
using Skimur.Embed;
using Skimur.Logging;
using Skimur.Markdown;
using Skimur.Messaging;
using Skimur.Messaging.Handling;
using Skimur.Settings;
using Skimur.Utils;
using Subs.Commands;
using Subs.Events;
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
        private readonly IEventBus _eventBus;
        private readonly IEmbeddedProvider _embeddedProvider;

        public PostHandler(IMarkdownCompiler markdownCompiler,
            ILogger<PostHandler> logger,
            IMembershipService membershipService,
            IPostService postService,
            ISubService subService,
            ISubUserBanService subUserBanService,
            ICommandBus commandBus,
            IPermissionService permissionService,
            ISettingsProvider<SubSettings> subSettings,
            IEventBus eventBus,
            IEmbeddedProvider embeddedProvider)
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
            _eventBus = eventBus;
            _embeddedProvider = embeddedProvider;
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

                List<string> mentions = null;

                if (post.PostType == PostType.Link)
                {
                    post.Url = command.Url;
                    post.Domain = domain;
                }
                else
                {
                    post.Content = command.Content;
                    post.ContentFormatted = _markdownCompiler.Compile(post.Content, out mentions);
                }

                _postService.InsertPost(post);
                _commandBus.Send(new CastVoteForPost { DateCasted = post.DateCreated, IpAddress = command.IpAddress, PostId = post.Id, UserId = user.Id, VoteType = VoteType.Up });

                if (mentions != null && mentions.Count > 0)
                    _eventBus.Publish(new UsersMentioned { PostId = post.Id, Users = mentions });

                _commandBus.Send(new GenerateThumbnailForPost { PostId = post.Id });

                if (_embeddedProvider.IsEnabled)
                    _commandBus.Send(new GenerateEmbeddedMediaObject { PostId = post.Id });

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

                List<string> oldMentions = null;
                List<string> newMentions = null;

                try
                {
                    _markdownCompiler.Compile(post.Content, out oldMentions);
                }
                catch (Exception ex)
                {
                    _logger.Error("There was an errror compiling previous mentions for a comment edit.", ex);
                }

                post.Content = command.Content;
                post.ContentFormatted = _markdownCompiler.Compile(post.Content, out newMentions);

                _postService.UpdatePost(post);

                if (oldMentions != null && oldMentions.Count > 0)
                {
                    // we have some mentions in our previous comment. let's see if they were removed
                    var removed = oldMentions.Except(newMentions).ToList();
                    if (removed.Count > 0)
                    {
                        _eventBus.Publish(new UsersUnmentioned
                        {
                            PostId = post.Id,
                            Users = removed
                        });
                    }
                }

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (newMentions != null)
                {
                    // the are some mentions in this comment.
                    // let's get only the new mentions that were previously in the comment
                    if (oldMentions != null && oldMentions.Count > 0)
                    {
                        newMentions = newMentions.Except(oldMentions).ToList();
                    }

                    if (newMentions.Count > 0)
                    {
                        _eventBus.Publish(new UsersMentioned
                        {
                            PostId = post.Id,
                            Users = newMentions
                        });
                    }
                }

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
                    var currentStickied = _postService.GetPosts(new List<Guid> { post.SubId }, sticky: true);
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
