namespace Tamaris.API.Hubs
{
    public interface IMessageHub
    {
        Task MessageSentToMe(string username);
        Task MessageSent(int messageId);
    }
}
