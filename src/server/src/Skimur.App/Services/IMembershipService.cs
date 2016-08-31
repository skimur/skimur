using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.App.Services
{
    public interface IMembershipService
    {
        Task<bool> UpdateUser(User user);
        
        Task<bool> InsertUser(User user);
        
        Task<bool> DeleteUser(Guid userId);
        
        Task<User> GetUserById(Guid userId);
        
        Task<User> GetUserByUserName(string userName);

        Task<User> GetUserByEmail(string emailAddress);

        Task<List<User>> GetUsersByIds(List<Guid> ids);

        Task<SeekedList<User>> GetAllUsers(int? skip = null, int? take = null);

        Task<bool> IsUserNameValid(string userName);

        Task<bool> IsEmailValid(string email);

        Task<bool> IsPasswordValid(string password);

        Task<bool> CanChangedEmail(Guid userId, string email);

        Task<Role> GetRoleById(Guid id);

        Task<Role> GetRoleByName(string roleName);

        Task<bool> UpdateRole(Role role);

        Task<bool> InsertRole(Role role);

        Task<bool> DeleteRole(Guid roleId);

        Task<IList<Role>> GetRoles();

        Task<bool> AddUserToRole(Guid userId, Guid roleId);

        Task<bool> RemoveUserFromRole(Guid userId, Guid roleId);

        Task<bool> IsInRole(Guid userId, Guid roleId);

        Task<IList<Role>> GetUserRoles(Guid userId);

        Task<UserValidationResult> ValidateUser(User user);

        Task ResetAccessFailedCount(Guid userId);

        Task AddRemoteLogin(Guid userId, string loginProvider, string loginKey);

        Task RemoveRemoteLogin(Guid userId, string loginProvider, string loginKey);

        Task<IList<UserLogin>> GetRemoteLoginsForUser(Guid userId);

        Task<User> FindUserByExternalLogin(string loginProvider, string loginKey);

        Task UpdateUserProfile(Guid userId, string fullName, string bio, string url, string location);
        
        Task UpdateUserAvatar(Guid userId, string avatarIdentifier);
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
