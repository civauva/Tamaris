using Tamaris.Entities.Admin;
using Tamaris.Domains.Admin;
using Tamaris.Domains.DataShaping;


namespace Tamaris.DAL.Interfaces.Admin
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
		Task<IEnumerable<UserForSelect>> GetAllForSelectAsync(CancellationToken cancellationToken = default);
		Task<PaginatedList<UserForSelect>> GetPaginatedForSelectAsync(QueryParameters parameters, string searchString, CancellationToken cancellationToken = default);
		Task<UserForSelect> GetForSelectByIdAsync(string username, CancellationToken cancellationToken = default);
		Task<UserForSelect> GetForSelectByUsernameAsync(string username, CancellationToken cancellationToken = default);
		Task<UserForSelect> GetForSelectByEmailAsync(string email, CancellationToken cancellationToken = default);

		#region Special methods (usually for nested Get API calls)

		Task<PaginatedList<UserForSelect>> GetPaginatedForSelect_ForRolesAsync(int Id, QueryParameters parameters, string searchString, CancellationToken cancellationToken = default);

        #endregion Special methods (usually for nested Get API calls)
    }
}
