using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Tamaris.Entities.Admin;
using Tamaris.Domains.Admin;
using Tamaris.Domains.DataShaping;



namespace Tamaris.DAL.Interfaces.Admin
{
	public interface IRoleRepository : IRepository<Role>
	{
		Task<IEnumerable<RoleForSelect>> GetAllForSelectAsync(CancellationToken cancellationToken = default);
		Task<PaginatedList<RoleForSelect>> GetPaginatedForSelectAsync(QueryParameters parameters, string searchString, CancellationToken cancellationToken = default);
		Task<RoleForSelect> GetForSelectAsync(string id, CancellationToken cancellationToken = default);

		#region Special methods (usually for nested Get API calls)

		Task<PaginatedList<RoleForSelect>> GetPaginatedForSelect_ForUsersAsync(int Id, QueryParameters parameters, string searchString, CancellationToken cancellationToken = default);

		#endregion Special methods (usually for nested Get API calls)
	}
}
