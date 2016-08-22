using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.App.Services.Impl
{
    public class MembershipService : IMembershipService
    {
        public bool UpdateUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool InsertUser(User user)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public User GetUserById(Guid userId)
        {
            throw new NotImplementedException();
        }

        public User GetUserByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        public User GetUserByEmail(string emailAddress)
        {
            throw new NotImplementedException();
        }

        public List<User> GetUsersByIds(List<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public SeekedList<User> GetAllUsers(int? skip = null, int? take = null)
        {
            throw new NotImplementedException();
        }

        public bool IsUserNameValid(string userName)
        {
            throw new NotImplementedException();
        }

        public bool IsEmailValid(string email)
        {
            throw new NotImplementedException();
        }

        public bool IsPasswordValid(string password)
        {
            throw new NotImplementedException();
        }

        public bool CanChangedEmail(Guid userId, string email)
        {
            throw new NotImplementedException();
        }

        public Role GetRoleById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Role GetRoleByName(string roleName)
        {
            throw new NotImplementedException();
        }

        public bool UpdateRole(Role role)
        {
            throw new NotImplementedException();
        }

        public bool InsertRole(Role role)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRole(Guid roleId)
        {
            throw new NotImplementedException();
        }

        public IList<Role> GetRoles()
        {
            throw new NotImplementedException();
        }

        public bool AddUserToRole(Guid userId, Guid roleId)
        {
            throw new NotImplementedException();
        }

        public bool RemoveUserFromRole(Guid userId, Guid roleId)
        {
            throw new NotImplementedException();
        }

        public bool IsInRole(Guid userId, Guid roleId)
        {
            throw new NotImplementedException();
        }

        public IList<Role> GetUserRoles(Guid userId)
        {
            throw new NotImplementedException();
        }

        public UserValidationResult ValidateUser(User user)
        {
            throw new NotImplementedException();
        }

        public void ResetAccessFailedCount(Guid userId)
        {
            throw new NotImplementedException();
        }

        public void AddRemoteLogin(Guid userId, string loginProvider, string loginKey)
        {
            throw new NotImplementedException();
        }

        public void RemoveRemoteLogin(Guid userId, string loginProvider, string loginKey)
        {
            throw new NotImplementedException();
        }

        public IList<UserLogin> GetRemoteLoginsForUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public User FindUserByExternalLogin(string loginProvider, string loginKey)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserProfile(Guid userId, string fullName, string bio, string url, string location)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserAvatar(Guid userId, string avatarIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
