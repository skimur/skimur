using System.Web;
using Infrastructure.Utils;
using Membership;
using Membership.Services;
using Microsoft.AspNet.Identity;

namespace Skimur.Web
{
    public class UserContext : IUserContext
    {
        private readonly IMembershipService _membershipService;
        private User _currentUser;

        public UserContext(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        public User CurrentUser
        {
            get
            {
                if (_currentUser != null) return _currentUser;

                if (!HttpContext.Current.Request.IsAuthenticated)
                    return null;

                _currentUser = _membershipService.GetUserById(HttpContext.Current.User.Identity.GetUserId().ParseGuid());

                return _currentUser;
            }
        }
    }
}
