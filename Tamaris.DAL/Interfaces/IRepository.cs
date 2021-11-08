using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Tamaris.Domains.DataShaping;


namespace Tamaris.DAL.Interfaces
{
	public interface IRepository<TEntity> where TEntity : class
	{
		Task<TEntity> GetAsync(int id, CancellationToken cancellationToken = default);
		Task<TEntity> GetAsync(Int64 id, CancellationToken cancellationToken = default);
		Task<TEntity> GetAsync(double id, CancellationToken cancellationToken = default);
		Task<TEntity> GetAsync(decimal id, CancellationToken cancellationToken = default);
		Task<TEntity> GetAsync(string id, CancellationToken cancellationToken = default);
		Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken = default);
		Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
		Task<PaginatedList<TEntity>> GetPaginatedAsync(QueryParameters parameters, CancellationToken cancellationToken = default);
		Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
		Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

		void Add(TEntity entity);
		void AddRange(IEnumerable<TEntity> entities);

		void Remove(TEntity entity);
		void RemoveRange(IEnumerable<TEntity> entities);
	}
}
