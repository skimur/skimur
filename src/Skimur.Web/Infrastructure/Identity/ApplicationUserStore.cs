using Membership;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Membership.Services;
using System.ComponentModel;

namespace Skimur.Web.Infrastructure.Identity
{
    public class ApplicationUserStore :
        IUserStore<User>,
        IRoleStore<Role>,
        IUserPasswordStore<User>,
        IPasswordHasher<User>,
        IUserValidator<User>
    {
        IMembershipService _membershipService;
        IPasswordManager _passwordManager;

        public ApplicationUserStore(IMembershipService membershipService,
            IPasswordManager passwordManager)
        {
            _membershipService = membershipService;
            _passwordManager = passwordManager;
        }

        #region IUserStore

        Task<IdentityResult> IUserStore<User>.CreateAsync(User user, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                _membershipService.InsertUser(user);
                return IdentityResult.Success;
            });
        }

        Task<IdentityResult> IUserStore<User>.DeleteAsync(User user, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                // TODO: ensure soft-delete
                _membershipService.DeleteUser(user.Id);
                return IdentityResult.Success;
            });
        }

        Task<User> IUserStore<User>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var id = ConvertIdFromString(userId);
                return _membershipService.GetUserById(id);
            });
        }

        Task<User> IUserStore<User>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                return _membershipService.GetUserByUserName(normalizedUserName.ToLower());
            });
        }

        Task<string> IUserStore<User>.GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.UserName))
                return Task.FromResult((string)null);

            return Task.FromResult(user.UserName.ToLower());
        }

        Task<string> IUserStore<User>.GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        Task<string> IUserStore<User>.GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        Task IUserStore<User>.SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = normalizedName;

            return Task.FromResult(0);
        }

        Task IUserStore<User>.SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;

            return Task.FromResult(0);
        }

        Task<IdentityResult> IUserStore<User>.UpdateAsync(User user, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                _membershipService.UpdateUser(user);
                return IdentityResult.Success;
            });
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

        public Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserPasswordStore

        Task IUserPasswordStore<User>.SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;

            return Task.FromResult(0);
        }

        Task<string> IUserPasswordStore<User>.GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        Task<bool> IUserPasswordStore<User>.HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        #endregion

        #region IPasswordHasher

        string IPasswordHasher<User>.HashPassword(User user, string password)
        {
            return _passwordManager.HashPassword(password);
        }

        PasswordVerificationResult IPasswordHasher<User>.VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            return _passwordManager.VerifyHashedPassword(hashedPassword, providedPassword) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        #endregion

        #region IUserValidator

        Task<IdentityResult> IUserValidator<User>.ValidateAsync(UserManager<User> manager, User user)
        {
            var validationResult = _membershipService.ValidateUser(user);

            if (validationResult == UserValidationResult.Success)
                return Task.FromResult(IdentityResult.Success);

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

            return Task.FromResult(IdentityResult.Failed(errors.Select(x => new IdentityError { Description = x }).ToArray()));
        }

        #endregion
        
        public virtual Guid ConvertIdFromString(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception("Invalid id");
            return (Guid)TypeDescriptor.GetConverter(typeof(Guid)).ConvertFromInvariantString(id);
        }

        public void Dispose()
        {

        }
    }
}
