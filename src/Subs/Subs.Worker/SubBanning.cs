using System;
using Infrastructure.Messaging.Handling;
using Membership.Services;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker
{
    public class SubBanning :
        ICommandHandlerResponse<BanUserFromSub, BanUserFromSubResponse>,
        ICommandHandlerResponse<UnBanUserFromSub, UnBanUserFromSubResponse>,
        ICommandHandlerResponse<UpdateUserSubBan, UpdateUserSubBanResponse>
    {
        private readonly IPermissionService _permissionDao;
        private readonly IMembershipService _membershipService;
        private readonly ISubService _subService;
        private readonly ISubUserBanService _subUserBanService;

        public SubBanning(IPermissionService permissionDao,
            IMembershipService membershipService,
            ISubService subService,
            ISubUserBanService subUserBanService)
        {
            _permissionDao = permissionDao;
            _membershipService = membershipService;
            _subService = subService;
            _subUserBanService = subUserBanService;
        }

        public BanUserFromSubResponse Handle(BanUserFromSub command)
        {
            var response = new BanUserFromSubResponse();

            try
            {
                var user = command.UserId.HasValue ? _membershipService.GetUserById(command.UserId.Value) : _membershipService.GetUserByUserName(command.UserName);
                
                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var bannedBy = _membershipService.GetUserById(command.BannedBy);

                if (bannedBy == null)
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

                if (!_permissionDao.CanUserModerateSub(bannedBy, sub.Id))
                {
                    response.Error = "You are not authorized to ban.";
                    return response;
                }

                _subUserBanService.BanUserFromSub(sub.Id, user.Id, user.UserName, command.DateBanned, command.BannedBy, command.ReasonPrivate, command.ReasonPublic);

                return response;
            }
            catch (Exception ex)
            {
                // TODO: log error
                response.Error = "An unknown error occured.";
                return response;
            }
        }

        public UnBanUserFromSubResponse Handle(UnBanUserFromSub command)
        {
            var response = new UnBanUserFromSubResponse();

            try
            {
                var user = command.UserId.HasValue ? _membershipService.GetUserById(command.UserId.Value) : _membershipService.GetUserByUserName(command.UserName);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var unBannedBy = _membershipService.GetUserById(command.UnBannedBy);

                if (unBannedBy == null)
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

                if (!_permissionDao.CanUserModerateSub(unBannedBy, sub.Id))
                {
                    response.Error = "You are not authorized to ban.";
                    return response;
                }

                _subUserBanService.UnBanUserFromSub(sub.Id, user.Id);

                return response;
            }
            catch (Exception ex)
            {
                // TODO: log error
                response.Error = "An unknown error occured.";
                return response;
            }
        }

        public UpdateUserSubBanResponse Handle(UpdateUserSubBan command)
        {
            var response = new UpdateUserSubBanResponse();

            try
            {
                var user = command.UserId.HasValue ? _membershipService.GetUserById(command.UserId.Value) : _membershipService.GetUserByUserName(command.UserName);

                if (user == null)
                {
                    response.Error = "Invalid user.";
                    return response;
                }

                var updatedBy = _membershipService.GetUserById(command.UpdatedBy);

                if (updatedBy == null)
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

                if (!_permissionDao.CanUserModerateSub(updatedBy, sub.Id))
                {
                    response.Error = "You are not authorized to ban.";
                    return response;
                }

                _subUserBanService.UpdateSubBanForUser(sub.Id, user.Id, command.ReasonPrivate);

                response.UserId = user.Id;

                return response;
            }
            catch (Exception ex)
            {
                // TODO: log error
                response.Error = "An unknown error occured.";
                return response;
            }
        }
    }
}
