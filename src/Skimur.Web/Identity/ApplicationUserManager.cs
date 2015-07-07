using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Membership;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace Skimur.Web.Identity
{
    public class ApplicationUserManager : UserManager<ApplicationUser, Guid>, IPasswordHasher, IIdentityValidator<ApplicationUser>, IIdentityValidator<string>
    {
        private readonly IPasswordManager _passwordManager;
        private readonly IMembershipService _membershipService;

        public ApplicationUserManager(IUserStore<ApplicationUser, Guid> store,
            IPasswordManager passwordManager,
            IMembershipService membershipService,
            IIdentityMessageService identityMessageService,
            IDataProtectionProvider dataProtectionProvider)
            : base(store)
        {
            _passwordManager = passwordManager;
            _membershipService = membershipService;

            EmailService = identityMessageService;
            UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser, Guid>(dataProtectionProvider.Create("UserToken"));

            PasswordHasher = this;
            UserValidator = this;
            PasswordValidator = this;

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;
        }

        public override Task<IdentityResult> ResetAccessFailedCountAsync(Guid userId)
        {
            return Task.Factory.StartNew(() =>
            {
                _membershipService.ResetAccessFailedCount(userId);
                return IdentityResult.Success;
            });
        }
        
        public string HashPassword(string password)
        {
            return _passwordManager.HashPassword(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return _passwordManager.VerifyHashedPassword(hashedPassword, providedPassword) ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }

        public Task<IdentityResult> ValidateAsync(ApplicationUser item)
        {
            return Task.Factory.StartNew(() =>
            {
                var validationResult = _membershipService.ValidateUser(item);

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

                return IdentityResult.Failed(errors.ToArray());
            });
        }

        public Task<IdentityResult> ValidateAsync(string password)
        {
            return Task.Factory.StartNew(() =>
            {
                if (!_membershipService.IsPasswordValid(password))
                    return IdentityResult.Failed("The password is not valid or is not strong enough.");
                return IdentityResult.Success;
            });
        }
    }
}