using Tamaris.Domains.Admin;

namespace Tamaris.Web.Services
{
    public interface IAdminDataService
    {
        Task<IEnumerable<UserForSelect>> GetAllUsers();
        Task<UserForSelect> GetUserById(int userId);
        Task<UserForSelect> GetUserByUsername(string userName);
        Task<UserForSelect> AddUser(UserForInsert user);
        Task<UserForSelect> DeleteUser(string username);
    }
}