using Dapper;
using Skimur.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.App.Services.Impl
{
    public class MembershipService : IMembershipService
    {
        private IDbConnectionProvider _conn;
        private IEntityService _entityService;

        public MembershipService(IDbConnectionProvider conn, IEntityService entityService)
        {
            _conn = conn;
            _entityService = entityService;
        }

        public Task AddRemoteLogin(Guid userId, string loginProvider, string loginKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddUserToRole(Guid userId, Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanChangedEmail(Guid userId, string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteRole(Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindUserByExternalLogin(string loginProvider, string loginKey)
        {
            throw new NotImplementedException();
        }

        public Task<SeekedList<User>> GetAllUsers(int? skip = default(int?), int? take = default(int?))
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLogin>> GetRemoteLoginsForUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Role> GetRoleById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Role> GetRoleByName(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Role>> GetRoles()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByEmail(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserById(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByUserName(string userName)
        {
            return Task.FromResult<User>(null);
        }

        public Task<IList<Role>> GetUserRoles(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsersByIds(List<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertRole(Role role)
        {
            throw new NotImplementedException();
        }
        
        public Task<bool> InsertUser(User user)
        {
            if (user.Id != Guid.Empty)
                throw new Exception("You cannot insert a user with an existing user id");

            user.Id = GuidUtil.NewSequentialId();
            user.CreatedDate = Common.CurrentTime();

            return _conn.Perform(async conn =>
            {
                return (await _entityService.Insert(user, conn)) == 1;
            });
        }

        public Task<bool> IsEmailValid(string email)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRole(Guid userId, Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsPasswordValid(string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserNameValid(string userName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRemoteLogin(Guid userId, string loginProvider, string loginKey)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveUserFromRole(Guid userId, Guid roleId)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCount(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRole(Role role)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAvatar(Guid userId, string avatarIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserProfile(Guid userId, string fullName, string bio, string url, string location)
        {
            throw new NotImplementedException();
        }

        public Task<UserValidationResult> ValidateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
