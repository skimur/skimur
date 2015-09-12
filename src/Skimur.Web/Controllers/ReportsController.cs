using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Infrastructure.Logging;
using Infrastructure.Messaging;
using Subs.Commands;

namespace Skimur.Web.Controllers
{
    public class ReportsController : BaseController
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly ICommandBus _commandBus;
        private readonly IUserContext _userContext;

        public ReportsController(ILogger<ReportsController> logger,
            ICommandBus commandBus,
            IUserContext userContext)
        {
            _logger = logger;
            _commandBus = commandBus;
            _userContext = userContext;
        }

        public ActionResult ReportComment(Guid commentId, ReasonType type, string reason)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to report."
                });
            }

            try
            {
                reason = BuildReasonFromType(type, reason);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }

            try
            {
                _commandBus.Send(new ReportComment
                {
                    ReportBy = _userContext.CurrentUser.Id,
                    CommentId = commentId,
                    Reason = reason
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Error reporting comment.", ex);
                return Json(new
                {
                    success = false,
                    error = "An unknown error occured."
                });
            }

            return Json(new
            {
                success = true,
                error = (string)null
            });
        }

        public ActionResult ReportPost(Guid postId, ReasonType type, string reason)
        {
            if (!Request.IsAuthenticated)
            {
                return Json(new
                {
                    success = false,
                    error = "You must be logged in to report."
                });
            }

            try
            {
                reason = BuildReasonFromType(type, reason);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message
                });
            }

            try
            {
                _commandBus.Send(new ReportPost
                {
                    ReportBy = _userContext.CurrentUser.Id,
                    PostId = postId,
                    Reason = reason
                });
            }
            catch (Exception ex)
            {
                _logger.Error("Error reporting comment.", ex);
                return Json(new
                {
                    success = false,
                    error = "An unknown error occured."
                });
            }

            return Json(new
            {
                success = true,
                error = (string)null
            });
        }

        private string BuildReasonFromType(ReasonType type, string reason)
        {
            if (string.IsNullOrEmpty(reason))
                reason = string.Empty;

            switch (type)
            {
                case ReasonType.Spam:
                    reason = "Spam";
                    break;
                case ReasonType.VoteManipulation:
                    reason = "Vote manipulation";
                    break;
                case ReasonType.PersonalInformation:
                    reason = "Personal information";
                    break;
                case ReasonType.SexualizingMinors:
                    reason = "Sexualizing minors";
                    break;
                case ReasonType.BreakingSkimur:
                    reason = "Breaking skimur";
                    break;
                case ReasonType.Other:
                    if (string.IsNullOrEmpty(reason))
                        throw new Exception("You must provide a reason.");
                    if (reason.Length > 200)
                        throw new Exception("The reason must not be greater than 200 characters.");
                    break;
                default:
                    throw new Exception("unknown type");
            }

            return reason;
        }

        public enum ReasonType
        {
            Spam = 0,
            VoteManipulation = 1,
            PersonalInformation = 2,
            SexualizingMinors = 3,
            BreakingSkimur = 4,
            Other = 5
        }
    }
}
