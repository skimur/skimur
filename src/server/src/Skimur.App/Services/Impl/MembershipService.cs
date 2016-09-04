using Dapper;
using ServiceStack.OrmLite;
using Skimur.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Skimur.App.Services.Impl
{
    public class MembershipService : IMembershipService
    {
        private IDbConnectionProvider _conn;
        private IPasswordManager _passwordManager;
        private Regex _emailRegex = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
        private Regex _usernameRegex = new Regex(@"^[a-zA-Z0-9]{1}[A-Za-z0-9-_]{1,19}$");

        public MembershipService(IDbConnectionProvider conn, IPasswordManager passwordManager)
        {
            _conn = conn;
            _passwordManager = passwordManager;
        }

        public async Task AddRemoteLogin(Guid userId, string loginProvider, string loginKey)
        {
            await _conn.Perform(async conn =>
            {
                if (await conn.CountAsync<UserLogin>(
                        x => x.UserId == userId && x.LoginProvider == loginProvider && x.LoginKey == loginKey) == 0)
                {
                    await conn.InsertAsync(new UserLogin
                    {
                        Id = GuidUtil.NewSequentialId(),
                        UserId = userId,
                        LoginProvider = loginProvider,
                        LoginKey = loginKey
                    });
                }
            });
        }

        public async Task<bool> AddUserToRole(Guid userId, Guid roleId)
        {
            long dbResult = 0;
            await _conn.Perform(async conn =>
            {
                if (await conn.CountAsync(conn.From<UserRole>().Where(x => x.UserId == userId && x.RoleId == roleId)) == 0)
                    dbResult = await conn.InsertAsync(new UserRole { RoleId = roleId, UserId = userId });
            });
            return dbResult == 1;
        }

        public async Task<bool> CanChangedEmail(Guid userId, string email)
        {
            // Verify passing of regular expression test.
            if (!await IsEmailValid(email)) return false;

            if (userId != Guid.Empty)
            {
                // Verify the email isn't taken by another user.
                return await _conn.Perform(async conn => (await conn.CountAsync<User>(user =>
                    (user.Id != userId)
                    && user.Email.ToLower() == email.ToLower())) == 0);
            }

            return await _conn.Perform(async conn => (await conn.CountAsync<User>(user =>
                user.Email.ToLower() == email.ToLower()) == 0));
        }

        public async Task<bool> DeleteRole(Guid roleId)
        {
            var dbResult = 0;
            if (roleId == Guid.Empty) throw new ArgumentNullException("roleId");
            await _conn.Perform(async conn =>
            {
                await conn.DeleteAsync<UserRole>(x => x.RoleId == roleId);
                dbResult = await conn.DeleteByIdAsync<Role>(roleId);
            });
            return dbResult == 1;
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            return await _conn.Perform(async conn =>
            {
                await conn.DeleteAsync<UserRole>(x => x.UserId == userId);
                return await conn.DeleteAsync<User>(user => user.Id == userId) == 1;
            });
        }

        public async Task<User> FindUserByExternalLogin(string loginProvider, string loginKey)
        {
            return await _conn.Perform(async conn =>
                await conn.SingleAsync(conn.From<User>()
                    .LeftJoin<UserLogin>((user, login) => user.Id == login.UserId)
                    .Where<UserLogin>(x => x.LoginProvider == loginProvider && x.LoginKey == loginKey)));
        }

        public async Task<SeekedList<User>> GetAllUsers(int? skip = default(int?), int? take = default(int?))
        {
            return await _conn.Perform(async conn =>
            {
                var query = conn.From<User>();

                var totalCount = await conn.CountAsync(query);

                query.Skip(skip).Take(take);

                return new SeekedList<User>(await conn.SelectAsync(query), skip ?? 0, take, totalCount);
            });
        }

        public async Task<IList<UserLogin>> GetRemoteLoginsForUser(Guid userId)
        {
            return userId == Guid.Empty 
                ? new List<UserLogin>() 
                : await _conn.Perform(async conn => await conn.SelectAsync<UserLogin>(x => x.UserId == userId));
        }

        public async Task<Role> GetRoleById(Guid id)
        {
            return id == Guid.Empty 
                ? null 
                : await _conn.Perform(async conn => await conn.SingleByIdAsync<Role>(id));
        }

        public async Task<Role> GetRoleByName(string roleName)
        {
            return string.IsNullOrEmpty(roleName) 
                ? null 
                : await _conn.Perform(async conn => await conn.SingleAsync<Role>(x => x.Name == roleName));
        }

        public async Task<IList<Role>> GetRoles()
        {
            return await _conn.Perform(async conn => await conn.SelectAsync<Role>());
        }

        public async Task<User> GetUserByEmail(string emailAddress)
        {
            return string.IsNullOrEmpty(emailAddress)
                ? null
                : await _conn.Perform(async con => await con.SingleAsync<User>(x => x.Email.ToLower() == emailAddress.ToLower()));
        }

        public async Task<User> GetUserById(Guid userId)
        {
            return userId == Guid.Empty
               ? null
               : await _conn.Perform(async conn => await conn.SingleByIdAsync<User>(userId));
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            return string.IsNullOrEmpty(userName)
              ? null
              : await _conn.Perform(async conn => await conn.SingleAsync<User>(x => x.UserName.ToLower() == userName.ToLower()));
        }

        public async Task<IList<Role>> GetUserRoles(Guid userId)
        {
            return await _conn.Perform(async conn => await conn.SelectAsync<Role>(conn.From<UserRole>()
                .LeftJoin<Role, UserRole>((role, userRole) => role.Id == userRole.RoleId)
                .Where(x => x.UserId == userId)));
        }

        public async Task<List<User>> GetUsersByIds(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<User>();

            return await _conn.Perform(async conn => await conn.SelectAsync(conn.From<User>().Where(x => ids.Contains(x.Id))));
        }

        public async Task<bool> InsertRole(Role role)
        {
            if (role.Id != Guid.Empty) throw new Exception("A role with a predetermined unique ID can not be created. The Id will be generated automatically.");
            role.Id = GuidUtil.NewSequentialId();
            return await _conn.Perform(async conn => await conn.InsertAsync(role)) == 1;
        }
        
        public Task<bool> InsertUser(User user)
        {
            if (user.Id != Guid.Empty)
                throw new Exception("You cannot insert a user with an existing user id");

            user.Id = GuidUtil.NewSequentialId();
            user.CreatedDate = Common.CurrentTime();

            return _conn.Perform(async conn =>
            {
                return (await conn.InsertAsync(user)) == 1;
            });
        }

        public Task<bool> IsEmailValid(string email)
        {
            if (string.IsNullOrEmpty(email)) return Task.FromResult(false);
            return Task.FromResult(_emailRegex.IsMatch(email));
        }

        public async Task<bool> IsInRole(Guid userId, Guid roleId)
        {
            return await _conn.Perform(async conn => await conn.CountAsync(conn.From<UserRole>().Where(x => x.UserId == userId && x.RoleId == roleId)) > 0);
        }

        public Task<bool> IsPasswordValid(string password)
        {
            return Task.FromResult(_passwordManager.PasswordStrength(password) > PasswordScore.VeryWeak);
        }

        public Task<bool> IsUserNameValid(string userName)
        {
            if (string.IsNullOrEmpty(userName)) return Task.FromResult(false);
            return Task.FromResult(_usernameRegex.IsMatch(userName));
        }

        public async Task RemoveRemoteLogin(Guid userId, string loginProvider, string loginKey)
        {
            await _conn.Perform(async conn => await conn.DeleteAsync<UserLogin>(x => x.UserId == userId && x.LoginProvider == loginProvider && x.LoginKey == loginKey));
        }

        public async Task<bool> RemoveUserFromRole(Guid userId, Guid roleId)
        {
            return await _conn.Perform(async conn =>
                await conn.DeleteAsync(conn.From<UserRole>()
                    .Where(x => x.RoleId == roleId && x.UserId == userId))) == 1;
        }

        public async Task ResetAccessFailedCount(Guid userId)
        {
            if (userId == Guid.Empty)
                return;
            await _conn.Perform(async conn => await conn.UpdateAsync<User>(new { AccessFailedCount = 0 }, user => user.Id == userId));
        }

        public async Task<bool> UpdateRole(Role role)
        {
            if (role.Id == Guid.Empty) throw new Exception("Invalid role.");
            if (string.IsNullOrEmpty(role.Name)) throw new Exception("Invalid role name.");
            return await _conn.Perform(async conn => await conn.UpdateAsync(role)) == 1;
        }

        public async Task<bool> UpdateUser(User user)
        {
            if (user.Id == Guid.Empty)
                throw new Exception("You cannot update a user that doesn't have a user id");

            return await _conn.Perform(async conn => await conn.UpdateAsync(user) == 1);
        }

        public async Task UpdateUserAvatar(Guid userId, string avatarIdentifier)
        {
            await _conn.Perform(async conn =>
            {
                await conn.UpdateAsync<User>(new
                {
                    AvatarIdentifier = avatarIdentifier
                },
                x => x.Id == userId);
            });
        }

        public async Task UpdateUserProfile(Guid userId, string fullName, string bio, string url, string location)
        {
            await _conn.Perform(async conn =>
            {
                await conn.UpdateAsync<User>(new
                {
                    FullName = fullName,
                    Bio = bio,
                    Url = url,
                    Location = location
                },
                x => x.Id == userId);
            });
        }

        public async Task<UserValidationResult> ValidateUser(User user)
        {
            var result = UserValidationResult.Success;

            if (!await IsUserNameValid(user.UserName))
                result |= result | UserValidationResult.InvalidUserName;
            if (!string.IsNullOrEmpty(user.Email) && !await IsEmailValid(user.Email))
                result |= UserValidationResult.InvalidEmail;

            if (result != UserValidationResult.Success) return result;

            if (!string.IsNullOrEmpty(user.Email) && !await CanChangedEmail(user.Id, user.Email))
                result |= UserValidationResult.EmailInUse;

            if (user.Id == Guid.Empty)
            {
                // we are inserting this user, let's see if any user has this username
                if (await GetUserByUserName(user.UserName) != null)
                    result |= UserValidationResult.UserNameInUse;
            }
            else
            {
                // we are updating the user, make sure the user name wasn't changed
                var dbUser = await GetUserByUserName(user.UserName);
                if (dbUser != null)
                    if (dbUser.Id != user.Id)
                        result |= UserValidationResult.CantChangeUsername;
            }

            return result;
        }
    }
}
