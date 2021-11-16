namespace Tamaris.API.Hubs
{
    public interface IMessageHub
    {
        Task MessageSentToMe();
        Task MessageSent(int messageId);
    }
}
