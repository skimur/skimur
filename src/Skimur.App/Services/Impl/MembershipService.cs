using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ServiceStack.OrmLite;
using Skimur.Data;
using Skimur.Utils;

namespace Skimur.App.Services.Impl
{
    public class MembershipService : IMembershipService
    {
        private readonly IDbConnectionProvider _conn;
        private readonly IPasswordManager _passwordManager;
        private Regex _emailRegex = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
        private Regex _usernameRegex = new Regex(@"^[a-zA-Z0-9]{1}[A-Za-z0-9-_]{1,19}$");

        public MembershipService(IDbConnectionProvider conn, IPasswordManager passwordManager)
        {
            _conn = conn;
            _passwordManager = passwordManager;
        }

        public virtual bool UpdateUser(User user)
        {
            if (user.Id == Guid.Empty)
                throw new Exception("You cannot update a user that doesn't have a user id");

            return _conn.Perform(conn => conn.Update(user) == 1);
        }

        public virtual bool InsertUser(User user)
        {
            if (user.Id != Guid.Empty)
                throw new Exception("You cannot insert a user with an existing user id");

            user.Id = GuidUtil.NewSequentialId();
            user.CreatedDate = Common.CurrentTime();
            return _conn.Perform(conn => conn.Insert(user) == 1);
        }

        public virtual bool DeleteUser(Guid userId)
        {
            return _conn.Perform(conn =>
            {
                conn.Delete<UserRole>(x => x.UserId == userId);
                return conn.Delete<User>(user => user.Id == userId) == 1;
            });
        }

        public virtual User GetUserById(Guid userId)
        {
            return userId == Guid.Empty
               ? null
               : _conn.Perform(conn => conn.SingleById<User>(userId));
        }

        public virtual User GetUserByUserName(string userName)
        {
            return string.IsNullOrEmpty(userName)
              ? null
              : _conn.Perform(conn => conn.Single<User>(x => x.UserName.ToLower() == userName.ToLower()));
        }

        public virtual User GetUserByEmail(string emailAddress)
        {
            return string.IsNullOrEmpty(emailAddress)
                ? null
                : _conn.Perform(con => con.Single<User>(x => x.Email.ToLower() == emailAddress.ToLower()));
        }
       
        /// <summary>
        /// Get a list of users by ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual List<User> GetUsersByIds(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<User>();

            return _conn.Perform(conn => conn.Select(conn.From<User>().Where(x => ids.Contains(x.Id))));
        }

        /// <summary>
        /// All the users in the system
        /// </summary>
        /// <returns></returns>
        public SeekedList<User> GetAllUsers(int? skip = null, int? take = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<User>();

                var totalCount = conn.Count(query);

                query.Skip(skip).Take(take);
                
                return new SeekedList<User>(conn.Select(query), skip ?? 0, take, totalCount);
            });
        }

        public virtual bool IsUserNameValid(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return false;
            return _usernameRegex.IsMatch(userName);
        }

        public virtual bool IsEmailValid(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;
            return _emailRegex.IsMatch(email);
        }

        public virtual bool IsPasswordValid(string password)
        {
            return _passwordManager.PasswordStrength(password) > PasswordScore.VeryWeak;
        }

        public virtual bool CanChangedEmail(Guid userId, string email)
        {
            // Verify passing of regular expression test.
            if (!IsEmailValid(email)) return false;

            if (userId != Guid.Empty)
            {
                // Verify the email isn't taken by another user.
                return _conn.Perform(conn => conn.Count<User>(user =>
                    (user.Id != userId)
                    && user.Email.ToLower() == email.ToLower()) == 0);
            }

            return _conn.Perform(conn => conn.Count<User>(user =>
                user.Email.ToLower() == email.ToLower())) == 0;
        }

        public virtual Role GetRoleById(Guid id)
        {
            return id == Guid.Empty ? null : _conn.Perform(conn => conn.SingleById<Role>(id));
        }

        public virtual Role GetRoleByName(string roleName)
        {
            return string.IsNullOrEmpty(roleName) ? null : _conn.Perform(conn => conn.Single<Role>(x => x.Name == roleName));
        }

        public virtual bool UpdateRole(Role role)
        {
            if (role.Id == Guid.Empty) throw new Exception("Invalid role.");
            if (string.IsNullOrEmpty(role.Name)) throw new Exception("Invalid role name.");
            return _conn.Perform(conn => conn.Update(role)) == 1;
        }

        public virtual bool InsertRole(Role role)
        {
            if (role.Id != Guid.Empty) throw new Exception("A role with a predetermined unique ID can not be created. The Id will be generated automatically.");
            role.Id = GuidUtil.NewSequentialId();
            return _conn.Perform(conn => conn.Insert(role)) == 1;
        }

        public virtual bool DeleteRole(Guid roleId)
        {
            var dbResult = 0;
            if (roleId == Guid.Empty) throw new ArgumentNullException("roleId");
            _conn.Perform(conn =>
            {
                conn.Delete<UserRole>(x => x.RoleId == roleId);
                dbResult = conn.DeleteById<Role>(roleId);
            });
            return dbResult == 1;
        }

        public virtual IList<Role> GetRoles()
        {
            return _conn.Perform(conn => conn.Select<Role>());
        }

        public virtual bool AddUserToRole(Guid userId, Guid roleId)
        {
            long dbResult = 0;
            _conn.Perform(conn =>
            {
                if (conn.Count(conn.From<UserRole>().Where(x => x.UserId == userId && x.RoleId == roleId)) == 0)
                    dbResult = conn.Insert(new UserRole { RoleId = roleId, UserId = userId });
            });
            return dbResult == 1;
        }

        public virtual bool RemoveUserFromRole(Guid userId, Guid roleId)
        {
            return _conn.Perform(conn =>
                conn.Delete(conn.From<UserRole>()
                    .Where(x => x.RoleId == roleId && x.UserId == userId))) == 1;
        }

        public virtual bool IsInRole(Guid userId, Guid roleId)
        {
            return _conn.Perform(conn => conn.Count(conn.From<UserRole>().Where(x => x.UserId == userId && x.RoleId == roleId)) > 0);
        }

        public virtual IList<Role> GetUserRoles(Guid userId)
        {
            return _conn.Perform(conn => conn.Select<Role>(conn.From<UserRole>()
                .LeftJoin<Role, UserRole>((role, userRole) => role.Id == userRole.RoleId)
                .Where(x => x.UserId == userId)));
        }

        /// <summary>
        /// Validate the user (update or insert)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual UserValidationResult ValidateUser(User user)
        {
            var result = UserValidationResult.Success;

            if (!IsUserNameValid(user.UserName))
                result |= result | UserValidationResult.InvalidUserName;
            if (!string.IsNullOrEmpty(user.Email) && !IsEmailValid(user.Email))
                result |= UserValidationResult.InvalidEmail;

            if (result != UserValidationResult.Success) return result;

            if (!string.IsNullOrEmpty(user.Email) && !CanChangedEmail(user.Id, user.Email))
                result |= UserValidationResult.EmailInUse;

            if (user.Id == Guid.Empty)
            {
                // we are inserting this user, let's see if any user has this username
                if (GetUserByUserName(user.UserName) != null)
                    result |= UserValidationResult.UserNameInUse;
            }
            else
            {
                // we are updating the user, make sure the user name wasn't changed
                var dbUser = GetUserByUserName(user.UserName);
                if (dbUser != null)
                    if (dbUser.Id != user.Id)
                        result |= UserValidationResult.CantChangeUsername;
            }

            return result;
        }

        /// <summary>
        /// Reset the access failed count for the user
        /// </summary>
        /// <param name="userId"></param>
        public virtual void ResetAccessFailedCount(Guid userId)
        {
            if (userId == Guid.Empty)
                return;
            _conn.Perform(conn => conn.Update<User>(new { AccessFailedCount = 0 }, user => user.Id == userId));
        }

        /// <summary>
        /// Add a remote login for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginProvider"></param>
        /// <param name="loginKey"></param>
        public virtual void AddRemoteLogin(Guid userId, string loginProvider, string loginKey)
        {
            _conn.Perform(conn =>
            {
                if (conn.Count<UserLogin>(
                        x => x.UserId == userId && x.LoginProvider == loginProvider && x.LoginKey == loginKey) == 0)
                {
                    conn.Insert(new UserLogin
                    {
                        Id = GuidUtil.NewSequentialId(),
                        UserId = userId,
                        LoginProvider = loginProvider,
                        LoginKey = loginKey
                    });
                }
            });
        }

        /// <summary>
        /// Remove a remote login from a user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginProvider"></param>
        /// <param name="loginKey"></param>
        public virtual void RemoveRemoteLogin(Guid userId, string loginProvider, string loginKey)
        {
            _conn.Perform(conn => conn.Delete<UserLogin>(x => x.UserId == userId && x.LoginProvider == loginProvider && x.LoginKey == loginKey));
        }

        /// <summary>
        /// Get logins for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual IList<UserLogin> GetRemoteLoginsForUser(Guid userId)
        {
            return userId == Guid.Empty ? new List<UserLogin>() : _conn.Perform(conn => conn.Select<UserLogin>(x => x.UserId == userId));
        }

        /// <summary>
        /// Find a user by an external login
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        public virtual User FindUserByExternalLogin(string loginProvider, string loginKey)
        {
            return _conn.Perform(conn =>
                conn.Single(conn.From<User>()
                    .LeftJoin<UserLogin>((user, login) => user.Id == login.UserId)
                    .Where<UserLogin>(x => x.LoginProvider == loginProvider && x.LoginKey == loginKey)));
        }

        /// <summary>
        /// Updates just profile data about a user
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="fullName">The full name.</param>
        /// <param name="bio">The bio.</param>
        /// <param name="url">The URL.</param>
        /// <param name="location">The location.</param>
        public virtual void UpdateUserProfile(Guid userId, string fullName, string bio, string url, string location)
        {
            _conn.Perform(conn =>
            {
                conn.Update<User>(new
                    {
                        FullName = fullName,
                        Bio = bio,
                        Url = url,
                        Location = location
                    },
                    x => x.Id == userId);
            });
        }

        /// <summary>
        /// Updates the user's avatar identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="avatarIdentifier">The avatar identifier.</param>
        public virtual void UpdateUserAvatar(Guid userId, string avatarIdentifier)
        {
            _conn.Perform(conn =>
            {
                conn.Update<User>(new
                    {
                        AvatarIdentifier = avatarIdentifier
                    },
                    x => x.Id == userId);
            });
        }
    }
}
