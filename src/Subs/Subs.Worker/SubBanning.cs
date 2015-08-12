using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Membership;
using Infrastructure.Messaging.Handling;
using Subs.Commands;
using Subs.ReadModel;
using Subs.Services;

namespace Subs.Worker
{
    public class SubBanning :
        ICommandHandlerResponse<BanUserFromSub, BanUserFromSubResponse>
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

                if (!_permissionDao.CanUserModerateSub(bannedBy.Id, sub.Id))
                {
                    response.Error = "You are not authorized to ban.";
                    return response;
                }

                _subUserBanService.BanUserFromSub(sub.Id, user.Id, user.UserName, command.BannedUntil,
                    command.DateBanned, command.BannedBy, command.ReasonPrivate, command.ReasonPublic);

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
