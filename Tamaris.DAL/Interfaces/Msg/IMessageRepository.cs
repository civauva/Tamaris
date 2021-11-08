using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Tamaris.Entities.Msg;
using Tamaris.Domains.Msg;
using Tamaris.Domains.DataShaping;



namespace Tamaris.DAL.Interfaces.Msg
{
	public interface IMessageRepository : IRepository<Message>
	{
		Task<IEnumerable<MessageForSelect>> GetAllForSelectAsync(CancellationToken cancellationToken = default);
		Task<PaginatedList<MessageForSelect>> GetPaginatedForSelectAsync(QueryParameters parameters, string searchString, CancellationToken cancellationToken = default);
		Task<MessageForSelect> GetForSelectAsync(int id, CancellationToken cancellationToken = default);

		#region Special methods (usually for nested Get API calls)

		Task<PaginatedList<MessageForSelect>> GetPaginatedForSelect_ForReceiverUserAsync(string Id, QueryParameters parameters, string searchString, CancellationToken cancellationToken = default);
		Task<PaginatedList<MessageForSelect>> GetPaginatedForSelect_ForSenderUserAsync(string Id, QueryParameters parameters, string searchString, CancellationToken cancellationToken = default);

		#endregion Special methods (usually for nested Get API calls)
	}
}
