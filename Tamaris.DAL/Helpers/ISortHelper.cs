using System.Linq;

namespace Tamaris.DAL.Helpers
{
	public interface ISortHelper<T>
	{
		IQueryable<T> ApplySort(IQueryable<T> entities, string orderByQueryString);
	}
}