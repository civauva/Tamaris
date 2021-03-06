using System.Linq.Expressions;
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

        #region IQueryable MessageForChats
        private IQueryable<MessageForChat> MessageForChats => MessageForChatsWhere(null);

        // Method that allows us to specify full navigation properties tree where clause
        private IQueryable<MessageForChat> MessageForChatsWhere(Expression<Func<Message, bool>> where)
        {
            Expression<Func<Message, MessageForChat>> selector = q => new MessageForChat
            {
                Id = q.Id,
                SenderUsername = q.SenderUser != null ? q.SenderUser.UserName : "",
                SenderFirstName = q.SenderUser != null ? q.SenderUser.FirstName : "",
                SenderLastName = q.SenderUser != null ? q.SenderUser.LastName : "",
                SenderAvatar = q.SenderUser != null ? q.SenderUser.Avatar : null,
                ReceiverUsername = q.ReceiverUser != null ? q.ReceiverUser.UserName : "",
                ReceiverFirstName = q.ReceiverUser != null ? q.ReceiverUser.FirstName : "",
                ReceiverLastName = q.ReceiverUser != null ? q.ReceiverUser.LastName : "",
                ReceiverAvatar = q.ReceiverUser != null ? q.ReceiverUser.Avatar : null,
                MessageText = q.MessageText,
                SentOn = q.SentOn,
                IsRead = q.IsRead
            };

            var query = where != null ?
                TamarisDbContext.Messages.Where(where).Select(selector) :
                TamarisDbContext.Messages.Select(selector);

            return query;
        }
        #endregion IQueryable MessageForChats

        public async Task<IEnumerable<MessageForSelect>> GetAllForSelectAsync(CancellationToken cancellationToken = default)
        {
            var query = MessageForSelects;
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MessageForChat>> GetAllForChatAsync(CancellationToken cancellationToken = default)
        {
            var query = MessageForChats;

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MessageForChat>> GetAllBetweenAsync(string username1, string username2, int countLastMessages, CancellationToken cancellationToken)
        {
            var query = MessageForChatsWhere(m =>
            (
                m.ReceiverUser.UserName == username1 &&
                m.SenderUser.UserName == username2
            )
            ||
            (
                m.ReceiverUser.UserName == username2 &&
                m.SenderUser.UserName == username1
            )).
            OrderByDescending(m => m.SentOn).
            Take(countLastMessages);

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



        public async Task MarkReadAsync(string receiverEmail, string senderEmail)
        {
            var messages = await TamarisDbContext.Messages.Where(m => m.ReceiverUser.Email == receiverEmail && m.SenderUser.Email == senderEmail && m.IsRead == false).ToListAsync();

            if(messages != null && messages.Count > 0)
            {
                foreach (var message in messages)
                    message.IsRead = true;
            }
        }

        public async Task<int> GetCountUnreadMessagesAsync(string receiverUsername, string senderUsername, CancellationToken cancellationToken)
        {
            int res;

            if(!string.IsNullOrEmpty(senderUsername))
                res = await TamarisDbContext.Messages.CountAsync(m => m.ReceiverUser.UserName == receiverUsername &&
                m.SenderUser.UserName == senderUsername &&
                m.IsRead == false);
            else
                res = await TamarisDbContext.Messages.CountAsync(m => m.ReceiverUser.UserName == receiverUsername &&
                m.IsRead == false);

            return res;
        }

        public async Task<int> GetCountUnreadMessagesByEmailAsync(string receiverEmail, string senderEmail, CancellationToken cancellationToken)
        {
            int res;

            if (!string.IsNullOrEmpty(senderEmail))
                res = await TamarisDbContext.Messages.CountAsync(m => m.ReceiverUser.Email == receiverEmail &&
                m.SenderUser.Email == senderEmail &&
                m.IsRead == false);
            else
                res = await TamarisDbContext.Messages.CountAsync(m => m.ReceiverUser.Email == receiverEmail &&
                m.IsRead == false);

            return res;
        }
    }
}