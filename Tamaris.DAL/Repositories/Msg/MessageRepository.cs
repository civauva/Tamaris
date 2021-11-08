using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Tamaris.DAL.DbContexts;
using Tamaris.Domains.DataShaping;
using Tamaris.Entities.Msg;
using Tamaris.Domains.Msg;
using Tamaris.DAL.Interfaces.Msg;
using Tamaris.DAL.Infrastructure;


namespace Tamaris.DAL.Repositories.Msg
{
	public class MessageRepository : Repository<Message>, IMessageRepository
	{
		public MessageRepository(TamarisDbContext context) : base(context)
		{
		}

		public TamarisDbContext TamarisDbContext => Context as TamarisDbContext;

		private Expression<Func<Message, bool>> GetMessageWhereClause(string searchString)
		{
			if (string.IsNullOrEmpty(searchString))
				return null;

			searchString = searchString.ToLower();
			var isNumber = long.TryParse(searchString, out long searchNumber);
			var isDate = DateTime.TryParse(searchString, out DateTime searchDate);
			var isBoolean = bool.TryParse(searchString, out bool searchBoolean);

			Expression<Func<Message, bool>> where = q =>
				q.MessageText.ToLower().Contains(searchString) ||
				q.SenderUserId == searchString ||
				q.ReceiverUserId == searchString ||
				(isDate &&
					(q.SentOn == searchDate)) ||
				(isBoolean &&
					(q.IsRead == searchBoolean));

			return where;
		}

		#region Explicit ForSelect methods

		#region IQueryable MessageForSelects

		// "Plain" property, without conditions
		private IQueryable<MessageForSelect> MessageForSelects => MessageForSelectsWhere(null);

		// Method that allows us to specify full navigation properties tree where clause
		private IQueryable<MessageForSelect> MessageForSelectsWhere(Expression<Func<Message, bool>> where)
		{
			Expression<Func<Message, MessageForSelect>> selector = q => new MessageForSelect
			{
				Id = q.Id,
				SenderUserId = q.SenderUserId,
				ReceiverUserId = q.ReceiverUserId,
				MessageText = q.MessageText,
				SentOn = q.SentOn,
				IsRead = q.IsRead,

			};

			var query = where != null ?
				TamarisDbContext.Messages.Where(where).Select(selector) :
				TamarisDbContext.Messages.Select(selector);

			return query;
		}

		#endregion IQueryable MessageForSelects

		public async Task<IEnumerable<MessageForSelect>> GetAllForSelectAsync(CancellationToken cancellationToken = default)
		{
			var query = MessageForSelects;
			return await query.ToListAsync(cancellationToken);
		}

		public async Task<PaginatedList<MessageForSelect>> GetPaginatedForSelectAsync(QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			var query = string.IsNullOrEmpty(searchString) ? MessageForSelects : MessageForSelectsWhere(GetMessageWhereClause(searchString));

			ApplySorting(ref query, parameters.OrderBy);

			var count = await query.CountAsync(cancellationToken);
			var items = (parameters.PageIndex > 0 && parameters.PageSize > 0) ? 
				await query.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync(cancellationToken) : 
				await query.ToListAsync(cancellationToken); 

			return new PaginatedList<MessageForSelect>(items, count, parameters.PageIndex, parameters.PageSize);
		}

		public async Task<MessageForSelect> GetForSelectAsync(int id, CancellationToken cancellationToken = default)
		{
			return await MessageForSelects.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
		}

		#region Special methods (usually for nested Get API calls)

		public async Task<PaginatedList<MessageForSelect>> GetPaginatedForSelect_ForReceiverUserAsync(string Id, QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			Expression<Func<Message, bool>> whereId = p => p.ReceiverUserId == Id;
			Expression<Func<Message, bool>> whereSearch = GetMessageWhereClause(searchString);
			Expression<Func<Message, bool>> whereAll = whereId.And(whereSearch);

			var query = string.IsNullOrEmpty(searchString) ? MessageForSelectsWhere(whereId) : MessageForSelectsWhere(whereAll);

			ApplySorting(ref query, parameters.OrderBy);

			var count = await query.CountAsync(cancellationToken);
			var items = (parameters.PageIndex > 0 && parameters.PageSize > 0) ? 
				await query.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync(cancellationToken) : 
				await query.ToListAsync(cancellationToken); 

			return new PaginatedList<MessageForSelect>(items, count, parameters.PageIndex, parameters.PageSize);
		}

		public async Task<PaginatedList<MessageForSelect>> GetPaginatedForSelect_ForSenderUserAsync(string Id, QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			Expression<Func<Message, bool>> whereId = p => p.SenderUserId == Id;
			Expression<Func<Message, bool>> whereSearch = GetMessageWhereClause(searchString);
			Expression<Func<Message, bool>> whereAll = whereId.And(whereSearch);

			var query = string.IsNullOrEmpty(searchString) ? MessageForSelectsWhere(whereId) : MessageForSelectsWhere(whereAll);

			ApplySorting(ref query, parameters.OrderBy);

			var count = await query.CountAsync(cancellationToken);
			var items = (parameters.PageIndex > 0 && parameters.PageSize > 0) ? 
				await query.Skip((parameters.PageIndex - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync(cancellationToken) : 
				await query.ToListAsync(cancellationToken); 

			return new PaginatedList<MessageForSelect>(items, count, parameters.PageIndex, parameters.PageSize);
		}


		#endregion Special methods (usually for nested Get API calls)

		#endregion Explicit ForSelect methods

	}
}