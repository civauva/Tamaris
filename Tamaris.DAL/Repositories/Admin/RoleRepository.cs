using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Tamaris.DAL.DbContexts;
using Tamaris.Domains.DataShaping;
using Tamaris.Entities.Admin;
using Tamaris.Domains.Admin;
using Tamaris.DAL.Interfaces.Admin;
using Tamaris.DAL.Infrastructure;


namespace Tamaris.DAL.Repositories.Admin
{
	public class RoleRepository : Repository<Role>, IRoleRepository
	{
		public RoleRepository(TamarisDbContext context) : base(context)
		{
		}

		public TamarisDbContext TamarisDbContext => Context as TamarisDbContext;

		private Expression<Func<Role, bool>> GetRoleWhereClause(string searchString)
		{
			if (string.IsNullOrEmpty(searchString))
				return null;

			searchString = searchString.ToLower();
			var isNumber = long.TryParse(searchString, out long searchNumber);
			var isDate = DateTime.TryParse(searchString, out DateTime searchDate);
			var isBoolean = bool.TryParse(searchString, out bool searchBoolean);

			Expression<Func<Role, bool>> where = q =>
				q.Name.ToLower().Contains(searchString) || 
				(isBoolean && 
					(q.IsAdmin == searchBoolean));

			return where;
		}

		#region Explicit ForSelect methods

		#region IQueryable RoleForSelects

		// "Plain" property, without conditions
		private IQueryable<RoleForSelect> RoleForSelects => RoleForSelectsWhere(null);

		// Method that allows us to specify full navigation properties tree where clause
		private IQueryable<RoleForSelect> RoleForSelectsWhere(Expression<Func<Role, bool>> where)
		{
			Expression<Func<Role, RoleForSelect>> selector = q => new RoleForSelect
			{
				Id = q.Id,
				RoleName = q.Name,
				IsAdmin = q.IsAdmin,

			};

			var query = where != null ?
				TamarisDbContext.Roles.Where(where).Select(selector) :
				TamarisDbContext.Roles.Select(selector);

			return query;
		}

		#endregion IQueryable RoleForSelects

		public async Task<IEnumerable<RoleForSelect>> GetAllForSelectAsync(CancellationToken cancellationToken = default)
		{
			var query = RoleForSelects;
			return await query.ToListAsync(cancellationToken);
		}

		public async Task<PaginatedList<RoleForSelect>> GetPaginatedForSelectAsync(QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			var query = string.IsNullOrEmpty(searchString) ? RoleForSelects : RoleForSelectsWhere(GetRoleWhereClause(searchString));

			ApplySorting(ref query, parameters.OrderBy);

			var count = await query.CountAsync(cancellationToken);
			var items = (parameters.PageIndex > 0 && parameters.PageSize > 0) ? 
				await query.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync(cancellationToken) : 
				await query.ToListAsync(cancellationToken); 

			return new PaginatedList<RoleForSelect>(items, count, parameters.PageIndex, parameters.PageSize);
		}

		public async Task<RoleForSelect> GetForSelectAsync(string id, CancellationToken cancellationToken = default)
		{
			return await RoleForSelects.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
		}

		#region Special methods (usually for nested Get API calls)

		public async Task<PaginatedList<RoleForSelect>> GetPaginatedForSelect_ForUsersAsync(int Id, QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			//Expression<Func<Role, bool>> whereId = p => p.UserRoles.Any(q => q.Id == Id);
			//Expression<Func<Role, bool>> whereSearch = GetRoleWhereClause(searchString);
			//Expression<Func<Role, bool>> whereAll = whereId.And(whereSearch);

			//var query = string.IsNullOrEmpty(searchString) ? RoleForSelectsWhere(whereId) : RoleForSelectsWhere(whereAll);

			//ApplySorting(ref query, parameters.OrderBy);

			//var count = await query.CountAsync(cancellationToken);
			//var items = (parameters.PageIndex > 0 && parameters.PageSize > 0) ? 
			//	await query.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync(cancellationToken) : 
			//	await query.ToListAsync(cancellationToken); 

			//return new PaginatedList<RoleForSelect>(items, count, parameters.PageIndex, parameters.PageSize);

			return null;
		}


		#endregion Special methods (usually for nested Get API calls)

		#endregion Explicit ForSelect methods

	}
}