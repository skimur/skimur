using System;
using Infrastructure.Logging;
using Infrastructure.Messaging.Handling;
using Membership.Services;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker.Commands
{
    public class ModerationHandler :
        ICommandHandlerResponse<RemoveModFromSub, RemoveModFromSubResponse>,
        ICommandHandlerResponse<ChangeModPermissionsForSub, ChangeModPermissionsForSubResponse>,
        ICommandHandlerResponse<AddUserModToSub, AddUserModToSubResponse>
    {
        private readonly IModerationService _moderationService;
        private readonly IMembershipService _membershipService;
        private readonly ISubService _subService;
        private readonly ILogger<ModerationHandler> _logger;

        public ModerationHandler(IModerationService moderationService,
            IMembershipService membershipService,
            ISubService subService,
            ILogger<ModerationHandler> logger)
        {
            _moderationService = moderationService;
            _membershipService = membershipService;
            _subService = subService;
            _logger = logger;
        }

        public RemoveModFromSubResponse Handle(RemoveModFromSub command)
        {
            var response = new RemoveModFromSubResponse();

            try
            {
                var requestingUser = _membershipService.GetUserById(command.RequestingUser);
                if (requestingUser == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var userToRemove = _membershipService.GetUserById(command.UserToRemove);
                if (userToRemove == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                Sub sub = null;

                if (command.SubId.HasValue)
                    sub = _subService.GetSubById(command.SubId.Value);
                else if (!string.IsNullOrEmpty(command.SubName))
                    sub = _subService.GetSubByName(command.SubName);

                if (sub == null)
                {
                    response.Error = "Invalid sub.";
                    return response;
                }

                var userToRemoveModInfo = _moderationService.GetModeratorInfoForUserInSub(userToRemove.Id, sub.Id);
                if (userToRemoveModInfo == null)
                {
                    response.Error = "The user is not a mod of this sub.";
                    return response;
                }

                if (requestingUser.IsAdmin)
                {
                    _moderationService.RemoveModFromSub(userToRemove.Id, sub.Id);
                }
                else
                {
                    var requestingUserModInfo = _moderationService.GetModeratorInfoForUserInSub(requestingUser.Id, sub.Id);
                    if (requestingUserModInfo == null)
                    {
                        response.Error = "You are not a mod of this sub.";
                        return response;
                    }

                    if (
                        // if the user is removing himself (doable)
                        requestingUser.Id == userToRemove.Id ||
                        // or the requesting user is an admin
                        requestingUser.IsAdmin ||
                        // or the user has has "full" access and the user we are changing has is a newby.
                        (requestingUserModInfo.AddedOn <=
                         userToRemoveModInfo.AddedOn &&
                         requestingUserModInfo.Permissions.HasFlag(
                             ModeratorPermissions.All))
                        )
                    {
                        _moderationService.RemoveModFromSub(userToRemove.Id, sub.Id);
                    }
                    else
                    {
                        response.Error = "You are not permitted to remove the mod from this sub.";
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Error = "An unknown error occured.";
                _logger.Error("An error occured removing a user from sub.", ex);
            }

            return response;
        }

        public ChangeModPermissionsForSubResponse Handle(ChangeModPermissionsForSub command)
        {
            var response = new ChangeModPermissionsForSubResponse();

            try
            {
                // you can't change your own permissions
                if (command.RequestingUser == command.UserToChange)
                {
                    response.Error = "You can't change your own permissions.";
                    return response;
                }

                var requestingUser = _membershipService.GetUserById(command.RequestingUser);
                if (requestingUser == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var userToChange = _membershipService.GetUserById(command.UserToChange);
                if (userToChange == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                Sub sub = null;

                if (command.SubId.HasValue)
                    sub = _subService.GetSubById(command.SubId.Value);
                else if (!string.IsNullOrEmpty(command.SubName))
                    sub = _subService.GetSubByName(command.SubName);

                if (sub == null)
                {
                    response.Error = "Invalid sub.";
                    return response;
                }

                if (requestingUser.IsAdmin)
                {
                    // the user is an admin, or the user has has "full" access and the user we are changing has is a newby.
                    _moderationService.UpdateUserModPermissionForSub(userToChange.Id, sub.Id, command.Permissions);
                }
                else
                {
                    var requestingUserModInfo = _moderationService.GetModeratorInfoForUserInSub(requestingUser.Id, sub.Id);
                    if (requestingUserModInfo == null)
                    {
                        response.Error = "You are not a mod of the sub.";
                        return response;
                    }

                    var userToChangeModInfo = _moderationService.GetModeratorInfoForUserInSub(userToChange.Id, sub.Id);
                    if (userToChangeModInfo == null)
                    {
                        response.Error = "The user that you are attempting change permissions for is not a mod.";
                        return response;
                    }

                    if (requestingUser.IsAdmin ||
                        (requestingUserModInfo.AddedOn <= userToChangeModInfo.AddedOn &&
                         requestingUserModInfo.Permissions.HasFlag(ModeratorPermissions.All)))
                    {
                        // the user is an admin, or the user has has "full" access and the user we are changing has is a newby.
                        _moderationService.UpdateUserModPermissionForSub(userToChange.Id, sub.Id, command.Permissions);
                    }
                    else
                    {
                        response.Error = "You are not authorized to change this mod's permissions.";
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Error = "An unknown error occured.";
                _logger.Error("An error occured changing a mods permissions.", ex);
            }

            return response;
        }

        public AddUserModToSubResponse Handle(AddUserModToSub command)
        {
            var response = new AddUserModToSubResponse();

            try
            {
                var requestingUser = _membershipService.GetUserById(command.RequestingUser);
                if (requestingUser == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var userToAdd = _membershipService.GetUserById(command.UserToAdd);
                if (userToAdd == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                Sub sub = null;

                if (command.SubId.HasValue)
                    sub = _subService.GetSubById(command.SubId.Value);
                else if (!string.IsNullOrEmpty(command.SubName))
                    sub = _subService.GetSubByName(command.SubName);

                if (sub == null)
                {
                    response.Error = "Invalid sub.";
                    return response;
                }

                if (requestingUser.IsAdmin)
                {
                    // admins can do anything
                    _moderationService.AddModToSub(userToAdd.Id, sub.Id, command.Permissions, requestingUser.Id);
                    return response;
                }

                var requestingUserModInfo = _moderationService.GetModeratorInfoForUserInSub(requestingUser.Id, sub.Id);

                if (requestingUserModInfo == null)
                {
                    response.Error = "Requesting user is not a mod of the sub.";
                    return response;
                }

                var userToChangeModInfo = _moderationService.GetModeratorInfoForUserInSub(userToAdd.Id, sub.Id);
                if (userToChangeModInfo != null)
                {
                    response.Error = "The user to already a mod of the sub.";
                    return response;
                }

                _moderationService.AddModToSub(userToAdd.Id, sub.Id, command.Permissions, requestingUser.Id);
            }
            catch (Exception ex)
            {
                response.Error = "An unknown error occured.";
                _logger.Error("An error occured adding a user as a mod.", ex);
            }

            return response;
        }
    }
}
