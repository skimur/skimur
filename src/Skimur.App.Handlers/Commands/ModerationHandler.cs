using System;
using Membership.Services;
using Skimur.Logging;
using Skimur.Messaging.Handling;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker.Commands
{
    public class ModerationHandler :
        ICommandHandlerResponse<RemoveModFromSub, RemoveModFromSubResponse>,
        ICommandHandlerResponse<ChangeModPermissionsForSub, ChangeModPermissionsForSubResponse>,
        ICommandHandlerResponse<AddUserModToSub, AddUserModToSubResponse>,
        ICommandHandlerResponse<InviteModToSub, InviteModToSubResponse>,
        ICommandHandlerResponse<AcceptModInvitation, AcceptModInvitationResponse>,
        ICommandHandlerResponse<RemoveModInviteFromSub, RemoveModInviteFromSubResponse>,
        ICommandHandlerResponse<ChangeModInvitePermissions, ChangeModInvitePermissionsResponse>
    {
        private readonly IModerationService _moderationService;
        private readonly IModerationInviteService _moderationInviteService;
        private readonly IMembershipService _membershipService;
        private readonly ISubService _subService;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<ModerationHandler> _logger;

        public ModerationHandler(IModerationService moderationService,
            IModerationInviteService moderationInviteService,
            IMembershipService membershipService,
            ISubService subService,
            IPermissionService permissionService,
            ILogger<ModerationHandler> logger)
        {
            _moderationService = moderationService;
            _moderationInviteService = moderationInviteService;
            _membershipService = membershipService;
            _subService = subService;
            _permissionService = permissionService;
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

                var userToRemove = command.UserIdToRemove.HasValue
                   ? _membershipService.GetUserById(command.UserIdToRemove.Value)
                   : _membershipService.GetUserByUserName(command.UserNameToRemove);
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
                var requestingUser = _membershipService.GetUserById(command.RequestingUser);
                if (requestingUser == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }
                
                var userToChange = command.UserIdToChange.HasValue
                   ? _membershipService.GetUserById(command.UserIdToChange.Value)
                   : _membershipService.GetUserByUserName(command.UserNameToChange);
                if (userToChange == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                // you can't change your own permissions
                if (requestingUser.Id == userToChange.Id)
                {
                    response.Error = "You can't change your own permissions.";
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

                _moderationInviteService.RemoveModeratorInvite(userToAdd.Id, sub.Id); // just in case
                _moderationService.AddModToSub(userToAdd.Id, sub.Id, command.Permissions, requestingUser.Id);
            }
            catch (Exception ex)
            {
                response.Error = "An unknown error occured.";
                _logger.Error("An error occured adding a user as a mod.", ex);
            }

            return response;
        }

        public InviteModToSubResponse Handle(InviteModToSub command)
        {
            var response = new InviteModToSubResponse();

            try
            {
                var requestingUser = _membershipService.GetUserById(command.RequestingUserId);
                if (requestingUser == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var sub = command.SubId.HasValue
                    ? _subService.GetSubById(command.SubId.Value)
                    : _subService.GetSubByName(command.SubName);
                if (sub == null)
                {
                    response.Error = "Invalid sub.";
                    return response;
                }
                
                var permission = _permissionService.GetUserPermissionsForSub(requestingUser, sub.Id);

                if (!permission.HasValue || !permission.Value.HasFlag(ModeratorPermissions.All))
                {
                    response.Error = "You are not permitted to invite users to mod a sub.";
                    return response;
                }

                var user = command.UserId.HasValue
                   ? _membershipService.GetUserById(command.UserId.Value)
                   : _membershipService.GetUserByUserName(command.UserName);
                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var existingModPermissions = _moderationService.GetUserPermissionsForSub(user, sub.Id);
                if (existingModPermissions.HasValue)
                {
                    response.Error = "The user is already a moderator.";
                    return response;
                }

                var userInviteInfo = _moderationInviteService.GetModeratorInviteInfo(user.Id, sub.Id);
                if (userInviteInfo != null)
                {
                    response.Error = "The user is already invited.";
                    return response;
                }

                _moderationInviteService.AddInvite(user.Id, sub.Id, requestingUser.Id, command.Permissions);
            }
            catch (Exception ex)
            {
                response.Error = "An unknown error occured.";
                _logger.Error("An error occured inviting a mod to sub.", ex);
            }

            return response;
        }

        public AcceptModInvitationResponse Handle(AcceptModInvitation command)
        {
            var response = new AcceptModInvitationResponse();

            try
            {
                var user = command.UserId.HasValue
                   ? _membershipService.GetUserById(command.UserId.Value)
                   : _membershipService.GetUserByUserName(command.UserName);
                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var sub = command.SubId.HasValue
                   ? _subService.GetSubById(command.SubId.Value)
                   : _subService.GetSubByName(command.SubName);
                if (sub == null)
                {
                    response.Error = "Invalid sub.";
                    return response;
                }
                
                var userInviteInfo = _moderationInviteService.GetModeratorInviteInfo(user.Id, sub.Id);
                if (userInviteInfo == null)
                {
                    response.Error = "The user is not invited as a mod to the sub.";
                    return response;
                }

                // remove the invite and add the user as a mod.
                _moderationInviteService.RemoveModeratorInvite(user.Id, sub.Id);
                _moderationService.AddModToSub(user.Id, sub.Id, userInviteInfo.Permissions, userInviteInfo.InvitedBy);
            }
            catch (Exception ex)
            {
                response.Error = "An unknown error occured.";
                _logger.Error("An error occured accepting an invite to a sub.", ex);
            }

            return response;
        }

        public RemoveModInviteFromSubResponse Handle(RemoveModInviteFromSub command)
        {
            var response = new RemoveModInviteFromSubResponse();

            try
            {
                var user = command.UserId.HasValue
                   ? _membershipService.GetUserById(command.UserId.Value)
                   : _membershipService.GetUserByUserName(command.UserName);
                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var sub = command.SubId.HasValue
                   ? _subService.GetSubById(command.SubId.Value)
                   : _subService.GetSubByName(command.SubName);
                if (sub == null)
                {
                    response.Error = "Invalid sub.";
                    return response;
                }

                var requestingUser = _membershipService.GetUserById(command.RequestingUserId);
                if (requestingUser == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }
                
                if (requestingUser.Id == user.Id)
                {
                    // the user is removing their invite (deny).
                }
                else
                {
                    var permission = _permissionService.GetUserPermissionsForSub(requestingUser, sub.Id);
                    if (!permission.HasValue || !permission.Value.HasFlag(ModeratorPermissions.All))
                    {
                        response.Error = "You are not permitted to remove mod invites from a sub.";
                        return response;
                    }
                }

                var userInviteInfo = _moderationInviteService.GetModeratorInviteInfo(user.Id, sub.Id);
                if (userInviteInfo == null)
                {
                    response.Error = "The user is not invited as a mod to the sub.";
                    return response;
                }

                // remove the invite.
                _moderationInviteService.RemoveModeratorInvite(user.Id, sub.Id);
            }
            catch (Exception ex)
            {
                response.Error = "An unknown error occured.";
                _logger.Error("An error occured removing a mod invite from a sub.", ex);
            }

            return response;
        }

        public ChangeModInvitePermissionsResponse Handle(ChangeModInvitePermissions command)
        {
            var response = new ChangeModInvitePermissionsResponse();

            try
            {
                var requestingUser = _membershipService.GetUserById(command.RequestingUserId);
                if (requestingUser == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var sub = command.SubId.HasValue
                    ? _subService.GetSubById(command.SubId.Value)
                    : _subService.GetSubByName(command.SubName);
                if (sub == null)
                {
                    response.Error = "Invalid sub.";
                    return response;
                }

                var permission = _permissionService.GetUserPermissionsForSub(requestingUser, sub.Id);

                if (!permission.HasValue || !permission.Value.HasFlag(ModeratorPermissions.All))
                {
                    response.Error = "You are not permitted to change permissions for mod invites.";
                    return response;
                }

                var user = command.UserId.HasValue
                   ? _membershipService.GetUserById(command.UserId.Value)
                   : _membershipService.GetUserByUserName(command.UserName);
                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var userInviteInfo = _moderationInviteService.GetModeratorInviteInfo(user.Id, sub.Id);
                if (userInviteInfo == null)
                {
                    response.Error = "The user is not current invited.";
                    return response;
                }
                
                _moderationInviteService.UpdateInvitePermissions(user.Id, sub.Id, command.Permissions);
            }
            catch (Exception ex)
            {
                response.Error = "An unknown error occured.";
                _logger.Error("An error occured changing an invited mod's permissions.", ex);
            }

            return response;
        }
    }
}
