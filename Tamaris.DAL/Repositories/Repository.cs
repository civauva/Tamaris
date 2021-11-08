using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Tamaris.Domains.DataShaping;
using Tamaris.DAL.Interfaces;
using Tamaris.DAL.Helpers;



namespace Tamaris.DAL.Repositories
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
	{
		protected readonly DbSet<TEntity> _entities;
		protected readonly DbContext Context;

		public Repository(DbContext context)
		{
			Context = context;
			_entities = context.Set<TEntity>();
		}


		protected void ApplySorting<T>(ref IQueryable<T> query, string orderBy)
		{
			if (string.IsNullOrEmpty(orderBy))
				return;

			var sortHelper = new SortHelper<T>();
			query = sortHelper.ApplySort(query, orderBy);
		}


		#region Getting
		public async Task<TEntity> GetAsync(int id, CancellationToken cancellationToken = default)
		{
			return await _entities.FindAsync( new object[] { id }, cancellationToken);
		}

		public async Task<TEntity> GetAsync(Int64 id, CancellationToken cancellationToken = default)
		{
			return await _entities.FindAsync( new object[] { id }, cancellationToken);
		}

		public async Task<TEntity> GetAsync(double id, CancellationToken cancellationToken = default)
		{
			return await _entities.FindAsync(new object[] { id }, cancellationToken);
		}

		public async Task<TEntity> GetAsync(decimal id, CancellationToken cancellationToken = default)
		{
			return await _entities.FindAsync(new object[] { id }, cancellationToken);
		}

		public async Task<TEntity> GetAsync(string id, CancellationToken cancellationToken = default)
		{
			return await _entities.FindAsync(new object[] { id }, cancellationToken);
		}
		
		public async Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _entities.FindAsync(new object[] { id }, cancellationToken);
		}

		public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await _entities.ToListAsync(cancellationToken);
		}

		public async Task<PaginatedList<TEntity>> GetPaginatedAsync(QueryParameters parameters, CancellationToken cancellationToken = default)
		{
			var count = await _entities.CountAsync(cancellationToken);
			var items = (parameters.PageIndex > 0 && parameters.PageSize > 0) ? 
				await _entities.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync() :
				await _entities.ToListAsync(cancellationToken);
			return new PaginatedList<TEntity>(items, count, parameters.PageIndex, parameters.PageSize);
		}


		public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
		{
			return await _entities.Where(predicate).ToListAsync(cancellationToken);
		}

		public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
		{
			return await _entities.SingleOrDefaultAsync(predicate, cancellationToken);
		}
		#endregion Getting



		#region Adding
		public void Add(TEntity entity)
		{
			_entities.Add(entity);
		}

		public void AddRange(IEnumerable<TEntity> entities)
		{
			_entities.AddRange(entities);
		}
		#endregion Adding



		#region Removing
		public void Remove(TEntity entity)
		{
			_entities.Remove(entity);
		}

		public void RemoveRange(IEnumerable<TEntity> entities)
		{
			_entities.RemoveRange(entities);
		}
		#endregion Removing
	}
}