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
	public class UserRepository : Repository<User>, IUserRepository
	{
		public UserRepository(TamarisDbContext context) : base(context)
		{
		}

		public TamarisDbContext TamarisDbContext => Context as TamarisDbContext;

		private Expression<Func<User, bool>> GetUserWhereClause(string searchString)
		{
			if (string.IsNullOrEmpty(searchString))
				return null;

			searchString = searchString.ToLower();
			var isNumber = long.TryParse(searchString, out long searchNumber);
			var isDate = DateTime.TryParse(searchString, out DateTime searchDate);
			var isBoolean = bool.TryParse(searchString, out bool searchBoolean);

			Expression<Func<User, bool>> where = q =>
				(q.UserName != null && q.UserName.ToLower().Contains(searchString)) || 
				(q.Email != null && q.Email.ToLower().Contains(searchString)) ||
				(q.FirstName != null && q.FirstName.ToLower().Contains(searchString)) ||
				(q.LastName != null && q.LastName.ToLower().Contains(searchString)) || 
				(q.Company != null && q.Company.ToLower().Contains(searchString));

			return where;
		}

		#region Explicit ForSelect methods

		#region IQueryable UserForSelects

		// "Plain" property, without conditions
		private IQueryable<UserForSelect> UserForSelects => UserForSelectsWhere(null);

		// Method that allows us to specify full navigation properties tree where clause
		private IQueryable<UserForSelect> UserForSelectsWhere(Expression<Func<User, bool>> where)
		{
			Expression<Func<User, UserForSelect>> selector = q => new UserForSelect
			{
				Id = q.Id,
				Username = q.UserName,
				FirstName = q.FirstName,
				LastName = q.LastName,
				Email = q.Email,
				Company = q.Company,
				Avatar = q.Avatar
			};

			var query = where != null ?
				TamarisDbContext.Users.Where(where).Select(selector) :
				TamarisDbContext.Users.Select(selector);

			return query;
		}

		#endregion IQueryable UserForSelects

		public async Task<IEnumerable<UserForSelect>> GetAllForSelectAsync(CancellationToken cancellationToken = default)
		{
			var query = UserForSelects;
			return await query.ToListAsync(cancellationToken);
		}

		public async Task<PaginatedList<UserForSelect>> GetPaginatedForSelectAsync(QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			var query = string.IsNullOrEmpty(searchString) ? UserForSelects : UserForSelectsWhere(GetUserWhereClause(searchString));

			ApplySorting(ref query, parameters.OrderBy);

			var count = await query.CountAsync(cancellationToken);
			var items = (parameters.PageIndex > 0 && parameters.PageSize > 0) ? 
				await query.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync(cancellationToken) : 
				await query.ToListAsync(cancellationToken); 

			return new PaginatedList<UserForSelect>(items, count, parameters.PageIndex, parameters.PageSize);
		}

		public async Task<UserForSelect> GetForSelectWithIdAsync(string id, CancellationToken cancellationToken = default)
		{
			return await UserForSelects.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
		}

		public async Task<UserForSelect> GetForSelectWithUsernameAsync(string userName, CancellationToken cancellationToken = default)
		{
			return await UserForSelects.FirstOrDefaultAsync(q => q.Username == userName, cancellationToken);
		}

		#region Special methods (usually for nested Get API calls)

		public async Task<PaginatedList<UserForSelect>> GetPaginatedForSelect_ForRolesAsync(int Id, QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			//Expression<Func<User, bool>> whereId = p => p.UserRoles.Any(q => q.Id == Id);
			//Expression<Func<User, bool>> whereSearch = GetUserWhereClause(searchString);
			//Expression<Func<User, bool>> whereAll = whereId.And(whereSearch);

			//var query = string.IsNullOrEmpty(searchString) ? UserForSelectsWhere(whereId) : UserForSelectsWhere(whereAll);

			//ApplySorting(ref query, parameters.OrderBy);

			//var count = await query.CountAsync(cancellationToken);
			//var items = (parameters.PageIndex > 0 && parameters.PageSize > 0) ? 
			//	await query.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync(cancellationToken) : 
			//	await query.ToListAsync(cancellationToken); 

			//return new PaginatedList<UserForSelect>(items, count, parameters.PageIndex, parameters.PageSize);

			return null;
		}


		#endregion Special methods (usually for nested Get API calls)

		#endregion Explicit ForSelect methods

	}
}