using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership.Services;
using Skimur.Logging;
using Skimur.Messaging.Handling;
using Subs.Commands;
using Subs.Services;

namespace Subs.Worker.Commands
{
    public class StylesHandler 
        : ICommandHandlerResponse<EditSubStylesCommand, EditSubStylesCommandResponse>
    {
        private readonly ILogger<StylesHandler> _logger;
        private readonly IMembershipService _membershipService;
        private readonly ISubService _subService;
        private readonly IPermissionService _permissionService;
        private readonly ISubCssService _subStylesService;

        public StylesHandler(ILogger<StylesHandler> logger,
            IMembershipService membershipService,
            ISubService subService,
            IPermissionService permissionService,
            ISubCssService subStylesService)
        {
            _logger = logger;
            _membershipService = membershipService;
            _subService = subService;
            _permissionService = permissionService;
            _subStylesService = subStylesService;
        }

        public EditSubStylesCommandResponse Handle(EditSubStylesCommand command)
        {
            var response = new EditSubStylesCommandResponse();

            try
            {
                var user = _membershipService.GetUserById(command.EditedByUserId);
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

                if (!_permissionService.CanUserManageSubStyles(user, sub.Id))
                {
                    response.Error = "You are not authorized to manage styles for this sub.";
                    return response;
                }

                var styles = _subStylesService.GetStylesForSub(sub.Id);
                if(styles == null) styles = new SubCss();

                styles.SubId = sub.Id;
                styles.CssType = command.CssType;
                styles.Embedded = command.Embedded;
                styles.ExternalCss = command.ExternalCss;
                styles.GitHubCssProjectName = command.GitHubCssProjectName;
                styles.GitHubCssProjectTag = command.GitHubCssProjectTag;
                styles.GitHubLessProjectName = command.GitHubLessProjectName;
                styles.GitHubLessProjectTag = command.GitHubLessProjectTag;

                _subStylesService.UpdateStylesForSub(styles);
            }
            catch (Exception ex)
            {
                _logger.Error("Error trying to edit a sub's styles.", ex);
                response.Error = "An unknown error occured.";
            }

            return response;
        }
    }
}
