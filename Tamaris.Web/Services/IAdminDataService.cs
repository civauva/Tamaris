using Tamaris.Domains.Admin;

namespace Tamaris.Web.Services
{
    public interface IAdminDataService
    {
        #region Users
        Task<IEnumerable<UserForSelect>> GetAllUsers();
        Task<IEnumerable<UserForChat>> GetAllUsersForMessaging(string excludeUsername);
        Task<UserForSelect> GetUserById(int userId);
        Task<UserForSelect> GetUserByUsernameAsync(string userName);
        Task<UserForSelect> GetUserByEmailAsync(string userName);
        Task<UserForSelect> AddUser(UserForInsert user);
        Task ModifyUser(UserForUpdate user);
        Task ModifyUserProfile(UserForProfileUpdate user);
        Task<UserForSelect> DeleteUser(string username);
        #endregion Users

        #region Roles
        Task<IEnumerable<RoleForSelect>> GetAllRoles();
        #endregion Roles
    }
}