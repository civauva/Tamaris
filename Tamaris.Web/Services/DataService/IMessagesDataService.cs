using Tamaris.Domains.Msg;

namespace Tamaris.Web.Services.DataService
{
    public interface IMessagesDataService
    {
        Task<IEnumerable<MessageForChat>> GetAllMessagesForChat();
        Task<IEnumerable<MessageForChat>> GetMessagesForChatBetween(string username1, string username2, int countLastMessages = 10);
        Task<int> GetUnreadCountAsync(string receiverUsername, string senderUsername = "");
        Task<int> GetUnreadCountByEmailAsync(string receiverEmail, string senderEmail = "");
        Task<MessageForSelect> AddMessage(MessageForInsert message);
        Task<MessageForSelect> DeleteMessage(int messageId);
        Task MarkMessagesRead(string receiverEmail, string senderEmail);
    }
}