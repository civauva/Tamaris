namespace Tamaris.API.Hubs
{
    public interface IMessageHub
    {
        Task MessageSentToMe(string senderEmail);
        Task MessagesRead(string senderEmail);
    }
}
