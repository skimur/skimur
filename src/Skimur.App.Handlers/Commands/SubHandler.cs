using System;
using System.Text.RegularExpressions;
using Skimur.App.Commands;
using Skimur.App.Events;
using Skimur.App.Services;
using Skimur.Markdown;
using Skimur.Messaging;
using Skimur.Messaging.Handling;
using Skimur.Settings;
using Skimur.Utils;

namespace Skimur.App.Handlers.Commands
{
    public class SubHandler :
        ICommandHandlerResponse<CreateSub, CreateSubResponse>,
        ICommandHandlerResponse<EditSub, EditSubResponse>,
        ICommandHandlerResponse<SubcribeToSub, SubcribeToSubResponse>,
        ICommandHandlerResponse<UnSubcribeToSub, UnSubcribeToSubResponse>
    {
        private readonly ISubService _subService;
        private readonly IMembershipService _membershipService;
        private readonly IPostService _postService;
        private readonly IEventBus _eventBus;
        private readonly ICommandBus _commandBus;
        private readonly ISubUserBanService _subUserBanService;
        private readonly IModerationService _moderationService;
        private readonly IPermissionService _permissionService;
        private readonly IMarkdownCompiler _markdownCompiler;
        private readonly ISettingsProvider<SubSettings> _subSettings;

        public SubHandler(ISubService subService,
            IMembershipService membershipService,
            IPostService postService,
            IEventBus eventBus,
            ICommandBus commandBus,
            ISubUserBanService subUserBanService,
            IModerationService moderationService,
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
            _moderationService = moderationService;
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

                if (!string.IsNullOrEmpty(command.SubmissionText) && command.SubmissionText.Length > 1000)
                {
                    response.Error = "The sidebar text cannot be greater than 1000 characters";
                    return response;
                }

                if (!string.IsNullOrEmpty(command.SidebarText) && command.SidebarText.Length > 3000)
                {
                    response.Error = "The submission text cannot be greater than 1000 characters";
                    return response;
                }
                
                var user = _membershipService.GetUserById(command.CreatedByUserId);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var userAccountAge = Common.CurrentTime() - user.CreatedDate;
                if (!user.IsAdmin && userAccountAge.TotalDays < _subSettings.Settings.MinUserAgeCreateSub)
                {
                    response.Error = "Your account is too new to create a sub.";
                    return response;
                }

                if (_subService.GetSubByName(command.Name) != null)
                {
                    response.Error = "The sub already exist.";
                    return response;
                }

                if (command.Name.Equals("random", StringComparison.InvariantCultureIgnoreCase)
                    || command.Name.Equals("all", StringComparison.InvariantCultureIgnoreCase)
                    || Common.IsReservedKeyword(command.Name))
                {
                    response.Error = "The name is invalid.";
                    return response;
                }

                // let's make sure the user creating this sub doesn't already have the maximum number of subs they are modding.
                var moddedSubsForUser = _moderationService.GetSubsModeratoredByUser(user.Id);
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
                    SidebarTextFormatted = _markdownCompiler.Compile(command.SidebarText),
                    SubmissionText = command.SubmissionText,
                    SubmissionTextFormatted = _markdownCompiler.Compile(command.SubmissionText),
                    SubType = command.Type,
                    CreatedBy = user.Id,
                    InAll = command.InAll,
                    Nsfw = command.Nsfw
                };

                // only admins can configure default subs
                if (user.IsAdmin && command.IsDefault.HasValue)
                    sub.IsDefault = command.IsDefault.Value;

                _subService.InsertSub(sub);

                response.SubId = sub.Id;
                response.SubName = sub.Name;

                _subService.SubscribeToSub(user.Id, sub.Id);
                _moderationService.AddModToSub(user.Id, sub.Id, ModeratorPermissions.All);
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

                if (!string.IsNullOrEmpty(command.SubmissionText) && command.SubmissionText.Length > 1000)
                {
                    response.Error = "The sidebar text cannot be greater than 1000 characters";
                    return response;
                }

                if (!string.IsNullOrEmpty(command.SidebarText) && command.SidebarText.Length > 3000)
                {
                    response.Error = "The submission text cannot be greater than 1000 characters";
                    return response;
                }

                // only admins can determine if a sub is a default sub
                if (user.IsAdmin && command.IsDefault.HasValue)
                    sub.IsDefault = command.IsDefault.Value;

                sub.Description = command.Description;
                sub.SidebarText = command.SidebarText;
                sub.SidebarTextFormatted = _markdownCompiler.Compile(command.SidebarText);
                sub.SubmissionText = command.SubmissionText;
                sub.SubmissionTextFormatted = _markdownCompiler.Compile(command.SubmissionText);
                sub.SubType = command.Type;
                sub.InAll = command.InAll;
                sub.Nsfw = command.Nsfw;

                _subService.UpdateSub(sub);
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
