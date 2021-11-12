using Tamaris.Entities.Msg;
using Tamaris.Domains.Msg;
using Tamaris.Domains.DataShaping;

namespace Tamaris.DAL.Interfaces.Msg
{
	public interface IMessageRepository : IRepository<Message>
	{
		Task<IEnumerable<MessageForSelect>> GetAllForSelectAsync(CancellationToken cancellationToken = default);
		Task<IEnumerable<MessageForChat>> GetAllForChatAsync(CancellationToken cancellationToken);
		Task<IEnumerable<MessageForChat>> GetAllBetweenAsync(string username1, string username2, int countLastMessages, CancellationToken cancellationToken);
		Task<PaginatedList<MessageForSelect>> GetPaginatedForSelectAsync(QueryParameters parameters, string searchString, CancellationToken cancellationToken = default);
		Task<MessageForSelect> GetForSelectAsync(int id, CancellationToken cancellationToken = default);
		Task MarkReadAsync(string receiverEmail, string senderEmail);
		Task<int> GetCountUnreadMessagesAsync(string receiverUsername, string senderUsername, CancellationToken cancellationToken);
		Task<int> GetCountUnreadMessagesByEmailAsync(string receiverEmail, string senderEmail, CancellationToken cancellationToken);

		#region Special methods (usually for nested Get API calls)

		Task<PaginatedList<MessageForSelect>> GetPaginatedForSelect_ForReceiverUserAsync(string Id, QueryParameters parameters, string searchString, CancellationToken cancellationToken = default);
		Task<PaginatedList<MessageForSelect>> GetPaginatedForSelect_ForSenderUserAsync(string Id, QueryParameters parameters, string searchString, CancellationToken cancellationToken = default);
        #endregion Special methods (usually for nested Get API calls)
    }
}
