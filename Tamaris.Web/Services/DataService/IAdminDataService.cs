using Tamaris.Domains.Admin;
using Tamaris.Domains.DataShaping;

namespace Tamaris.Web.Services.DataService
{
    public interface IAdminDataService
    {
        #region Users
        Task<Tuple<IEnumerable<UserForSelect>, PaginationHeader>> GetAllUsers(int pageNumber, int pageSize);
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