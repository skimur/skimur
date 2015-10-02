using System;
using System.Text.RegularExpressions;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;
using Infrastructure.Settings;
using Infrastructure.Utils;
using Membership.Services;
using Skimur;
using Skimur.Markdown;
using Subs.Commands;
using Subs.Events;
using Subs.Services;

namespace Subs.Worker.Commands
{
    public class SubHandler :
        ICommandHandlerResponse<CreateSub, CreateSubResponse>,
        ICommandHandlerResponse<EditSub, EditSubResponse>,
        ICommandHandlerResponse<CreatePost, CreatePostResponse>,
        ICommandHandlerResponse<SubcribeToSub, SubcribeToSubResponse>,
        ICommandHandlerResponse<UnSubcribeToSub, UnSubcribeToSubResponse>
    {
        private readonly ISubService _subService;
        private readonly IMembershipService _membershipService;
        private readonly IPostService _postService;
        private readonly IEventBus _eventBus;
        private readonly ICommandBus _commandBus;
        private readonly ISubUserBanService _subUserBanService;
        private readonly ISubModerationService _subModerationService;
        private readonly IPermissionService _permissionService;
        private readonly IMarkdownCompiler _markdownCompiler;
        private readonly ISettingsProvider<SubSettings> _subSettings;

        public SubHandler(ISubService subService,
            IMembershipService membershipService,
            IPostService postService,
            IEventBus eventBus,
            ICommandBus commandBus,
            ISubUserBanService subUserBanService,
            ISubModerationService subModerationService,
            IPermissionService permissionService,
            IMarkdownCompiler markdownCompiler,
            ISettingsProvider<SubSettings>  subSettings)
        {
            _subService = subService;
            _membershipService = membershipService;
            _postService = postService;
            _eventBus = eventBus;
            _commandBus = commandBus;
            _subUserBanService = subUserBanService;
            _subModerationService = subModerationService;
            _permissionService = permissionService;
            _markdownCompiler = markdownCompiler;
            _subSettings = subSettings;
        }

        public CreateSubResponse Handle(CreateSub command)
        {
            var response = new CreateSubResponse();

            try
            {
                if (string.IsNullOrEmpty(command.Name))
                {
                    response.Error = "Sub name is required.";
                    return response;
                }

                if (!Regex.IsMatch(command.Name, "^[a-zA-Z0-9]*$"))
                {
                    response.Error = "No spaces or special characters.";
                    return response;
                }

                if (command.Name.Length > 20)
                {
                    response.Error = "The name length is limited to 20 characters.";
                    return response;
                }

                if (string.IsNullOrEmpty(command.Description))
                {
                    response.Error = "Please describe your sub.";
                    return response;
                }

                var user = _membershipService.GetUserById(command.CreatedByUserId);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                if (_subService.GetSubByName(command.Name) != null)
                {
                    response.Error = "The sub already exist.";
                    return response;
                }
                
                // let's make sure the user creating this sub doesn't already have the maximum number of subs they are modding.
                var moddedSubsForUser = _subModerationService.GetSubsModeratoredByUser(user.Id);
                if (moddedSubsForUser.Count >= _subSettings.Settings.MaximumNumberOfModdedSubs)
                {
                    response.Error = "You can only moderate a maximum of " + _subSettings.Settings.MaximumNumberOfModdedSubs + " subs.";
                    return response;
                }

                var sub = new Sub
                {
                    Id = GuidUtil.NewSequentialId(),
                    CreatedDate = Common.CurrentTime(),
                    Name = command.Name,
                    Description = command.Description,
                    SidebarText = command.SidebarText,
                    SubType = command.Type,
                    CreatedBy = user.Id
                };

                // only admins can configure default subs
                if (user.IsAdmin && command.IsDefault.HasValue)
                    sub.IsDefault = command.IsDefault.Value;

                _subService.InsertSub(sub);

                response.SubId = sub.Id;
                response.SubName = sub.Name;

                _subService.SubscribeToSub(user.Id, sub.Id);
                _subModerationService.AddModToSub(user.Id, sub.Id);
            }
            catch (Exception ex)
            {
                // todo: log
                response.Error = ex.Message;
            }

            return response;
        }

        public EditSubResponse Handle(EditSub command)
        {
            var response = new EditSubResponse();

            try
            {
                var sub = _subService.GetSubByName(command.Name);

                if (sub == null)
                {
                    response.Error = "No sub found with the given name.";
                    return response;
                }

                var user = _membershipService.GetUserById(command.EditedByUserId);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                if (!_permissionService.CanUserManageSubConfig(user, sub.Id))
                {
                    response.Error = "You are not allowed to modify this sub.";
                    return response;
                }
                
                if (string.IsNullOrEmpty(command.Description))
                {
                    response.Error = "Please describe your sub.";
                    return response;
                }

                // only admins can determine if a sub is a default sub
                if (user.IsAdmin && command.IsDefault.HasValue)
                    sub.IsDefault = command.IsDefault.Value;

                sub.Description = command.Description;
                sub.SidebarText = command.SidebarText;
                sub.SubType = command.Type;

                _subService.UpdateSub(sub);
            }
            catch (Exception ex)
            {
                // todo: log
                response.Error = ex.Message;
            }

            return response;
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

                var post = new Post
                {
                    Id = GuidUtil.NewSequentialId(),
                    DateCreated = Common.CurrentTime(),
                    LastEditDate = null,
                    SubId = sub.Id,
                    UserId = user.Id,
                    UserIp = command.IpAddress,
                    PostType = command.PostType,
                    Title = command.Title,
                    SendReplies = command.NotifyReplies
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

        public SubcribeToSubResponse Handle(SubcribeToSub command)
        {
            var response = new SubcribeToSubResponse();

            try
            {
                var user = _membershipService.GetUserByUserName(command.UserName);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var sub = command.SubId.HasValue ? _subService.GetSubById(command.SubId.Value) : _subService.GetSubByName(command.SubName);
                
                if (sub == null)
                {
                    response.Error = "Invalid sub name.";
                    return response;
                }

                // todo: check for private subs
                // todo: check if the user is subcribed to too many subs

                var subscribedSubs = _subService.GetSubscribedSubsForUser(user.Id);

                if (subscribedSubs.Contains(sub.Id))
                {
                    // already subscribed!
                    response.Success = true;
                    return response;
                }

                if (subscribedSubs.Count >= _subSettings.Settings.MaximumNumberOfSubscribedSubs)
                {
                    response.Error = "You cannot have more than " + _subSettings.Settings.MaximumNumberOfSubscribedSubs + " subscribed subs.";
                    return response;
                }

                _subService.SubscribeToSub(user.Id, sub.Id);

                _eventBus.Publish(new SubScriptionChanged
                {
                    Subcribed = true,
                    UserId = user.Id,
                    SubId = sub.Id
                });

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = ex.Message;
                return response;
            }

            return response;
        }

        public UnSubcribeToSubResponse Handle(UnSubcribeToSub command)
        {
            var response = new UnSubcribeToSubResponse();

            try
            {
                var sub = _subService.GetSubByName(command.SubName);

                if (sub == null)
                {
                    response.Error = "No sub found with the given name";
                    return response;
                }

                _subService.UnSubscribeToSub(command.UserId, sub.Id);

                _eventBus.Publish(new SubScriptionChanged
                {
                    Unsubscribed = true,
                    UserId = command.UserId,
                    SubId = sub.Id
                });

                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Error = ex.Message;
                return response;
            }

            return response;
        }
    }
}
