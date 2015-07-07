using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Membership;
using Microsoft.AspNet.Identity;

namespace Skimur.Web.Identity
{
    public class ApplicationUserStore : 
        IUserStore<ApplicationUser, Guid>,
        IUserPasswordStore<ApplicationUser, Guid>,
        IUserEmailStore<ApplicationUser, Guid>,
        IUserLockoutStore<ApplicationUser, Guid>,
        IUserTwoFactorStore<ApplicationUser, Guid>, IUserRoleStore<ApplicationUser, Guid>,
        IUserPhoneNumberStore<ApplicationUser, Guid>,
        IUserLoginStore<ApplicationUser, Guid>
    {
        private readonly IMembershipService _membershipService;
        private readonly IMapper _mapper;
        
        public ApplicationUserStore(IMembershipService membershipService,
            IMapper mapper)
        {
            _membershipService = membershipService;
            _mapper = mapper;
        }

        #region IUserStore

        public Task CreateAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew(() => _membershipService.InsertUser(user));
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew(() => _membershipService.DeleteUser(user.Id));
        }

        public Task<ApplicationUser> FindByIdAsync(Guid userId)
        {
            return Task.Factory.StartNew(() => _mapper.Map<User, ApplicationUser>(_membershipService.GetUserById(userId)));
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return Task.Factory.StartNew(() => _mapper.Map<User, ApplicationUser>(_membershipService.GetUserByUserName(userName)));
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew(() => _membershipService.UpdateUser(user));
        }

        #endregion

        #region IUserPasswordStore

        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserEmailStore

        public Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return Task.Factory.StartNew(() => _mapper.Map<User, ApplicationUser>(_membershipService.GetUserByEmail(email)));
        }

        public Task<string> GetEmailAsync(ApplicationUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailAsync(ApplicationUser user, string email)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        #endregion

        #region Methods

        public void Dispose()
        {

        }

        #endregion

        #region IUserLockoutStore

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(ApplicationUser user)
        {
            return
               Task.FromResult(user.LockoutEndDate.HasValue
                   ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDate.Value, DateTimeKind.Utc))
                   : new DateTimeOffset());
        }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user)
        {
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(ApplicationUser user)
        {
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDate = lockoutEnd == DateTimeOffset.MinValue ? (DateTime?)null : lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserTwoFactorStore

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user)
        {
            return Task.FromResult(false);
        }

        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            throw new NotSupportedException();
        }

        #endregion


        #region IUserRoleStore

        public Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            return Task.Factory.StartNew(() =>
            {
                var role = _membershipService.GetRoleByName(roleName);
                if(role == null)
                    throw new Exception("Role " + roleName + " doesn't exist.");
                _membershipService.AddUserToRole(user.Id, role.Id);
            });
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew(() => (IList<string>)_membershipService.GetUserRoles(user.Id).Select(x => x.Name).ToList());
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            return Task.Factory.StartNew(() =>
            {
                var role = _membershipService.GetRoleByName(roleName);
                return role != null && _membershipService.IsInRole(user.Id, role.Id);
            });
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            return Task.Factory.StartNew(() =>
            {
                var role = _membershipService.GetRoleByName(roleName);
                if (role == null)
                    return;
                _membershipService.RemoveUserFromRole(user.Id, role.Id);
            });
        }

        #endregion

        #region IUserPhoneNumberStore

        public Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = true;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserLoginStore

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            return Task.Factory.StartNew(() =>
            {
                _membershipService.AddRemoteLogin(user.Id, login.LoginProvider, login.ProviderKey);
            });
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            return Task.Factory.StartNew(() =>
            {
                _membershipService.RemoveRemoteLogin(user.Id, login.LoginProvider, login.ProviderKey);
            });
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew(() =>
            {
                return (IList<UserLoginInfo>)_membershipService.GetRemoteLoginsForUser(user.Id).Select(x => new UserLoginInfo(x.LoginProvider, x.LoginKey)).ToList();
            });
        }

        public Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            return Task.Factory.StartNew(() =>
            {
                var result = _membershipService.FindUserByExternalLogin(login.LoginProvider, login.ProviderKey);
                if (result == null) return null as ApplicationUser;
                return _mapper.Map<User, ApplicationUser>(result);
            });
        }

        #endregion
    }
}
