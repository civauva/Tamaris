namespace Tamaris.API.Hubs
{
    public interface IMessageHub
    {
        Task MessageSentToMe(string senderEmail);
        Task MessageSent(int messageId);
    }
}
