using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur;

namespace Infrastructure.Membership
{
    public interface IMembershipService
    {
        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user.</param>
        bool UpdateUser(User user);

        /// <summary>
        /// Insert a new user
        /// </summary>
        /// <param name="user">The user.</param>
        bool InsertUser(User user);

        /// <summary>
        /// Deletes the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        bool DeleteUser(Guid userId);

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        User GetUserById(Guid userId);

        /// <summary>
        /// Gets the user by username.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <returns></returns>
        User GetUserByUserName(string userName);

        /// <summary>
        /// Gets the user by email.
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns></returns>
        User GetUserByEmail(string emailAddress);

        /// <summary>
        /// Get all the users from the system
        /// </summary>
        /// <param name="queryText">The query text.</param>
        /// <param name="skip">The number of items to skip.</param>
        /// <param name="take">The number of items to take.</param>
        /// <param name="inRole">Filter users by ones in the given role.</param>
        /// <returns></returns>
        SeekedList<User> GetUsers(string queryText = null, int? skip = null, int? take = null, Guid? inRole = null);

        /// <summary>
        /// Get a list of users by user names
        /// </summary>
        /// <param name="userNames"></param>
        /// <returns></returns>
        List<User> GetUsersByUserNames(List<string> userNames);
        
        /// <summary>
        /// Get a list of users by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        List<User> GetUsersByIds(List<Guid> ids); 

        /// <summary>
        /// Determines if the given username is a valid one. It does NOT check if it is currently used.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        bool IsUserNameValid(string userName);

        /// <summary>
        /// Determines if the given email is a valid one. It does NOT check if it is currently used.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        bool IsEmailValid(string email);

        /// <summary>
        /// Determines if the given password is a valid one.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        bool IsPasswordValid(string password);
        
        /// <summary>
        /// Can the given user id change their email to the provided email?
        /// null userId implies new user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        bool CanChangedEmail(Guid userId, string email);

        /// <summary>
        /// Retrieves a Role from the system.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Role GetRoleById(Guid id);

        /// <summary>
        /// Get a role by name
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        Role GetRoleByName(string roleName);

        /// <summary>
        /// Updates a pre-existing role, replacing the Name and Description.
        /// </summary>
        /// <param name="role"></param>
        bool UpdateRole(Role role);

        /// <summary>
        /// Creates a membership role in the system.
        /// </summary>
        /// <param name="role">The role to be created.</param>
        bool InsertRole(Role role);

        /// <summary>
        /// Removes a Role from the system.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        bool DeleteRole(Guid roleId);

        /// <summary>
        /// Gets all Roles from the system.
        /// </summary>
        /// <returns></returns>
        IList<Role> GetRoles();

        /// <summary>
        /// Adds a user to a role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        bool AddUserToRole(Guid userId, Guid roleId);
        
        /// <summary>
        /// Removes a user from a given role.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        bool RemoveUserFromRole(Guid userId, Guid roleId);
        
        /// <summary>
        /// Verifies whether a user is part of a given role
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns></returns>
        bool IsInRole(Guid userId, Guid roleId);

        /// <summary>
        /// Retrieves all the Roles a User possesses.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        IList<Role> GetUserRoles(Guid userId);

        /// <summary>
        /// Validate the user (update or insert)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        UserValidationResult ValidateUser(User user);

        /// <summary>
        /// Reset the access failed count for the user
        /// </summary>
        /// <param name="userId"></param>
        void ResetAccessFailedCount(Guid userId);

        /// <summary>
        /// Add a remote login for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginProvider"></param>
        /// <param name="loginKey"></param>
        void AddRemoteLogin(Guid userId, string loginProvider, string loginKey);
        
        /// <summary>
        /// Remove a remote login from a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginProvider"></param>
        /// <param name="loginKey"></param>
        void RemoveRemoteLogin(Guid userId, string loginProvider, string loginKey);

        /// <summary>
        /// Get logins for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IList<UserLogin> GetRemoteLoginsForUser(Guid userId);

        /// <summary>
        /// Find a user by an external login
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        User FindUserByExternalLogin(string loginProvider, string loginKey);

        /// <summary>
        /// Updates just profile data about a user
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fullName">The full name.</param>
        /// <param name="bio">The bio.</param>
        /// <param name="url">The URL.</param>
        /// <param name="location">The location.</param>
        void UpdateUserProfile(Guid userId, string fullName, string bio, string url, string location);

        /// <summary>
        /// Updates the user's avatar identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="avatarIdentifier">The avatar identifier.</param>
        void UpdateUserAvatar(Guid userId, string avatarIdentifier);
    }

    [Flags]
    public enum UserValidationResult
    {
        Success = 0,
        UnknownError = 1,
        InvalidUserName = 1 << 1,
        UserNameInUse = 1 << 2,
        CantChangeUsername = 1 << 3,
        InvalidEmail = 1 << 4,
        EmailInUse = 1 << 5
    }
}
