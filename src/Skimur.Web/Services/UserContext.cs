using Microsoft.AspNet.Http;
using Skimur.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Skimur.App;
using Skimur.App.Services;

namespace Skimur.Web.Services
{
    public class UserContext : IUserContext
    {
        private readonly IMembershipService _membershipService;
        private User _currentUser;
        private IHttpContextAccessor _httpContextAccessor;

        public UserContext(IMembershipService membershipService, IHttpContextAccessor httpContextAccessor)
        {
            _membershipService = membershipService;
            _httpContextAccessor = httpContextAccessor;
        }

        public User CurrentUser
        {
            get
            {
                if (_currentUser != null) return _currentUser;

                if (!_httpContextAccessor.HttpContext.User.IsSignedIn())
                    return null;

                _currentUser = _membershipService.GetUserById(_httpContextAccessor.HttpContext.User.GetUserId().ParseGuid());

                if (_currentUser == null)
                {
                    //_authenticationManager.SignOut();
                    throw new Exception("Auth cookie exists for an invalid user. UserId=" + _httpContextAccessor.HttpContext.User.GetUserId());
                }

                return _currentUser;
            }
        }

        public bool? CurrentNsfw
        {
            get
            {
                // anonymous users don't see NSFW content.
                // logged in users only see NSFW if preferences say so.
                // If they want to see NSFW, they will see all content (SFW/NSFW).
                return CurrentUser == null
                    ? false
                    : (CurrentUser.ShowNsfw ? (bool?)null : false);
            }
        }
    }
}
