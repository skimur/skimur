using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Skimur.App;
using Skimur.App.Services;

namespace Skimur.Web.Services
{
    public class MembershipStore : IUserStore<User>, IRoleStore<Role>, IUserPasswordStore<User>
    {
        IMembershipService _membershipService;

        public MembershipStore(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        #region IUserStore

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var result = await _membershipService.InsertUser(user);

            if (!result)
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Couldn't create user."
                });

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }
        
        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return _membershipService.GetUserByUserName(normalizedUserName);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            if (user.Id == null) return Task.FromResult((string)null);

            if (user.Id == Guid.Empty) return Task.FromResult((string)null);

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IRoleStore

        public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<Role> IRoleStore<Role>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<Role> IRoleStore<Role>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Dispose()
        {

        }

        #region IUserPassword

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        #endregion
    }
}
