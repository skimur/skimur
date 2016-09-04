using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Skimur.App;
using Skimur.App.Services;
using System.ComponentModel;

namespace Skimur.Web.Services
{
    public class MembershipStore : 
        IUserStore<User>, 
        IRoleStore<Role>, 
        IUserPasswordStore<User>, 
        IUserRoleStore<User>,
        IUserEmailStore<User>,
        IUserPhoneNumberStore<User>,
        IUserTwoFactorStore<User>,
        IUserValidator<User>,
        IPasswordHasher<User>,
        IUserLoginStore<User>
    {
        IMembershipService _membershipService;
        IPasswordManager _passwordManager;

        public MembershipStore(IMembershipService membershipService,
            IPasswordManager passwordManager)
        {
            _membershipService = membershipService;
            _passwordManager = passwordManager;
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

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            // TODO: ensure soft-delete
            var result = await _membershipService.DeleteUser(user.Id);

            if (!result)
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Couldn't delete user."
                });

            return IdentityResult.Success;
        }
        
        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var id = ConvertIdFromString(userId);
            return _membershipService.GetUserById(id);
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

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            if(await _membershipService.UpdateUser(user))
                return IdentityResult.Success;

            return IdentityResult.Failed(new IdentityError
            {
                Description = "Couldn't update user."
            });
        }

        #endregion

        #region IRoleStore

        public Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        Task<Role> IRoleStore<Role>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        Task<Role> IRoleStore<Role>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        #endregion
        
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

        #region IUserRoleStore

        public Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            var result = new List<string>();
            if (user.IsAdmin)
                result.Add("Admin");
            return Task.FromResult((IList<string>)result);
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentNullException(nameof(roleName));
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(user.IsAdmin);

            return Task.FromResult(false);
        }

        public Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IUserEmailStore

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return _membershipService.GetUserByEmail(normalizedEmail);
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.Email = normalizedEmail;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserPhoneNumberStore

        public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        #endregion

        #region IUserTwoFactorStore

        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        #endregion

        #region IUserValidator

        public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            var validationResult = await _membershipService.ValidateUser(user);

            if (validationResult == UserValidationResult.Success)
                return IdentityResult.Success;

            var errors = new List<string>();

            if (validationResult.HasFlag(UserValidationResult.UnknownError))
            {
                errors.Add("Unknown error updating user.");
            }
            if (validationResult.HasFlag(UserValidationResult.InvalidUserName))
            {
                errors.Add("Invalid user name.");
            }
            if (validationResult.HasFlag(UserValidationResult.UserNameInUse))
            {
                errors.Add("The user name is already in use.");
            }
            if (validationResult.HasFlag(UserValidationResult.CantChangeUsername))
            {
                errors.Add("The user name cannot be changed.");
            }
            if (validationResult.HasFlag(UserValidationResult.InvalidEmail))
            {
                errors.Add("The email is invalid.");
            }
            if (validationResult.HasFlag(UserValidationResult.EmailInUse))
            {
                errors.Add("The email has already been registered.");
            }

            return IdentityResult.Failed(errors.Select(x => new IdentityError { Description = x }).ToArray());
        }

        #endregion

        #region IPasswordHasher

        public string HashPassword(User user, string password)
        {
            return _passwordManager.HashPassword(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            return _passwordManager.VerifyHashedPassword(hashedPassword, providedPassword) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        #endregion

        #region Methods

        public virtual Guid ConvertIdFromString(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception("Invalid id");
            return (Guid)TypeDescriptor.GetConverter(typeof(Guid)).ConvertFromInvariantString(id);
        }

        public void Dispose()
        {

        }

        #endregion

        #region IUserLoginStore

        public Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            return _membershipService.AddRemoteLogin(user.Id, login.LoginProvider, login.ProviderKey);
        }

        public Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return _membershipService.RemoveRemoteLogin(user.Id, loginProvider, providerKey);
        }

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken cancellationToken)
        {
            var logins = (await _membershipService.GetRemoteLoginsForUser(user.Id))
                .Select(x => new UserLoginInfo(x.LoginProvider, x.LoginKey, x.LoginProvider /*todo*/))
                .ToList();
            return logins;
        }

        public Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return _membershipService.FindUserByExternalLogin(loginProvider, providerKey);
        }

        #endregion
    }
}
