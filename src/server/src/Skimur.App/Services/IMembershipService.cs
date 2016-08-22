using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.App.Services
{
    public interface IMembershipService
    {
        bool UpdateUser(User user);
        
        bool InsertUser(User user);
        
        bool DeleteUser(Guid userId);
        
        User GetUserById(Guid userId);
        
        User GetUserByUserName(string userName);
        
        User GetUserByEmail(string emailAddress);
        
        List<User> GetUsersByIds(List<Guid> ids);
        
        SeekedList<User> GetAllUsers(int? skip = null, int? take = null);
        
        bool IsUserNameValid(string userName);
        
        bool IsEmailValid(string email);
        
        bool IsPasswordValid(string password);
        
        bool CanChangedEmail(Guid userId, string email);
        
        Role GetRoleById(Guid id);
        
        Role GetRoleByName(string roleName);
        
        bool UpdateRole(Role role);
        
        bool InsertRole(Role role);
        
        bool DeleteRole(Guid roleId);
        
        IList<Role> GetRoles();
        
        bool AddUserToRole(Guid userId, Guid roleId);
        
        bool RemoveUserFromRole(Guid userId, Guid roleId);
        
        bool IsInRole(Guid userId, Guid roleId);
        
        IList<Role> GetUserRoles(Guid userId);
        
        UserValidationResult ValidateUser(User user);
        
        void ResetAccessFailedCount(Guid userId);
        
        void AddRemoteLogin(Guid userId, string loginProvider, string loginKey);
        
        void RemoveRemoteLogin(Guid userId, string loginProvider, string loginKey);
        
        IList<UserLogin> GetRemoteLoginsForUser(Guid userId);
        
        User FindUserByExternalLogin(string loginProvider, string loginKey);
        
        void UpdateUserProfile(Guid userId, string fullName, string bio, string url, string location);
        
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
