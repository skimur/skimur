using Infrastructure.Messaging.Handling;
using Membership.Services;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker.Commands
{
    public class ModerationHandler :
        ICommandHandler<RemoveModFromSub>,
        ICommandHandler<ChangeModPermissionsForSub>
    {
        private readonly ISubModerationService _subModerationService;
        private readonly IMembershipService _membershipService;
        private readonly ISubService _subService;

        public ModerationHandler(ISubModerationService subModerationService,
            IMembershipService membershipService,
            ISubService subService)
        {
            _subModerationService = subModerationService;
            _membershipService = membershipService;
            _subService = subService;
        }

        public void Handle(RemoveModFromSub command)
        {
            var requestingUser = _membershipService.GetUserById(command.RequestingUser);
            if (requestingUser == null) return;

            var userToRemove = _membershipService.GetUserById(command.UserToRemove);
            if (userToRemove == null) return;

            Sub sub = null;

            if (command.SubId.HasValue)
                sub = _subService.GetSubById(command.SubId.Value);
            else if (!string.IsNullOrEmpty(command.SubName))
                sub = _subService.GetSubByName(command.SubName);

            if (sub == null) return;

            var requestingUserModInfo = _subModerationService.GetModeratorInfoForUserInSub(requestingUser.Id, sub.Id);
            if (requestingUserModInfo == null) return;

            var userToRemoveModInfo = _subModerationService.GetModeratorInfoForUserInSub(userToRemove.Id, sub.Id);
            if (userToRemoveModInfo == null) return;
            
            if
            (
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
                _subModerationService.RemoveModFromSub(userToRemove.Id, sub.Id);
            }
        }

        public void Handle(ChangeModPermissionsForSub command)
        {
            // you can't change your own permissions
            if (command.RequestingUser == command.UserToChange) return;

            var requestingUser = _membershipService.GetUserById(command.RequestingUser);
            if (requestingUser == null) return;

            var userToChange = _membershipService.GetUserById(command.UserToChange);
            if (userToChange == null) return;

            Sub sub = null;

            if (command.SubId.HasValue)
                sub = _subService.GetSubById(command.SubId.Value);
            else if (!string.IsNullOrEmpty(command.SubName))
                sub = _subService.GetSubByName(command.SubName);

            if (sub == null) return;

            var requestingUserModInfo = _subModerationService.GetModeratorInfoForUserInSub(requestingUser.Id, sub.Id);
            if (requestingUserModInfo == null) return;

            var userToChangeModInfo = _subModerationService.GetModeratorInfoForUserInSub(userToChange.Id, sub.Id);
            if (userToChangeModInfo == null) return;

            if (requestingUser.IsAdmin ||
                (requestingUserModInfo.AddedOn <= userToChangeModInfo.AddedOn &&
                 requestingUserModInfo.Permissions.HasFlag(ModeratorPermissions.All)))
            {
                // the user it an admin, or the user has has "full" access and the user we are changing has is a newby.
                _subModerationService.UpdateUserModPermissionForSub(userToChange.Id, sub.Id, command.Permissions);
            }
        }
    }
}
