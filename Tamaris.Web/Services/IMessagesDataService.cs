using Tamaris.Domains.Msg;

namespace Tamaris.Web.Services
{
    public interface IMessagesDataService
    {
        Task<IEnumerable<MessageForChat>> GetAllMessagesForChat();
        Task<IEnumerable<MessageForChat>> GetMessagesForChatBetween(string username1, string username2, int countLastMessages = 10);
        Task<MessageForSelect> AddMessage(MessageForInsert message);
        Task<MessageForSelect> DeleteMessage(int messageId);
        Task MarkMessagesRead(IEnumerable<int> messageIds);
    }
}