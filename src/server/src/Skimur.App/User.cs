using ServiceStack.DataAnnotations;
using System;

namespace Skimur.App
{
    /// <summary>
    /// Represents a user of the system
    /// </summary>
    [Alias("Users")]
    public class User : IUserSettings
    {
        public User()
        {
            // defaults
            EnableStyles = true;
        }

        /// <summary>
        /// User ID (Primary Key)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The date the user was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// True if the email is confirmed, default is false
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// The salted/hashed form of the user password
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// A random value that should change whenever a users credentials have changed (password changed, login removed)
        /// </summary>
        public string SecurityStamp { get; set; }

        /// <summary>
        /// PhoneNumber for the user
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// True if the phone number is confirmed, default is false
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Is two factor enabled for the user
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// DateTime in UTC when lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public DateTime? LockoutEndDate { get; set; }

        /// <summary>
        /// Is lockout enabled for this user
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Used to record failures for the purposes of lockout
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// Security question for resetting password
        /// </summary>
        public string SecurityQuestion { get; set; }

        /// <summary>
        /// Security answer for resetting password
        /// </summary>
        public string SecurityAnswer { get; set; }

        /// <summary>
        /// The full name of the user
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// A short bio for the user
        /// </summary>
        public string Bio { get; set; }

        /// <summary>
        /// A url for the user. Maybe a blog?
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Where the user is located.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or set the avatar to use for the user
        /// </summary>
        public string AvatarIdentifier { get; set; }

        /// <summary>
        /// Is this user an administrator?
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Does the user want to see NSFW content?
        /// </summary>
        public bool ShowNsfw { get; set; }

        /// <summary>
        /// Does the user want to be presented with any custom styles?
        /// </summary>
        [Alias("Styles")]
        public bool EnableStyles { get; set; }

        /// <summary>
        /// The IP address used when registering this account.
        /// </summary>
        public string Ip { get; set; }
    }
}
