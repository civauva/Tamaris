using System.Net.Http.Json;
using System.Text.Json;
using Tamaris.Domains.Msg;

namespace Tamaris.Web.Services.DataService
{
    public class MessagesDataService : BaseDataService, IMessagesDataService
    {
        public MessagesDataService(HttpClient httpClient): base(httpClient) { }

        public async Task<IEnumerable<MessageForChat>> GetAllMessagesForChat()
        {
            return await GetResultAsync<IEnumerable<MessageForChat>>($"Msg/Messages/ForChat");
        }


        public async Task<IEnumerable<MessageForChat>> GetMessagesForChatBetween(string username1, string username2, int countLastMessages = 5)
        {
            var res = await GetResultAsync<IEnumerable<MessageForChat>>($"Msg/Messages/Conversation?username1={username1}&username2={username2}&countLastMessages={countLastMessages}");
            return res ?? new List<MessageForChat>();
        }

        public async Task<int> GetUnreadCountAsync(string receiverUsername, string senderUsername = "")
        {
            return await JsonSerializer.DeserializeAsync<int>
                (await _httpClient.GetStreamAsync($"Msg/Messages/CountUnread?receiverUsername={receiverUsername}&senderUsername={senderUsername}"), _options);
        }

        public async Task<int> GetUnreadCountByEmailAsync(string receiverEmail, string senderEmail = "")
        {
            return await JsonSerializer.DeserializeAsync<int>
                (await _httpClient.GetStreamAsync($"Msg/Messages/CountUnread/ByEmail?receiverEmail={receiverEmail}&senderEmail={senderEmail}"), _options);
        }


        public async Task<MessageForSelect> AddMessage(MessageForInsert message)
        {
            var registrationResult = await _httpClient.PostAsJsonAsync("Msg/Messages", message);
            var registrationContent = await registrationResult.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<MessageForSelect>(registrationContent, _options);
            return result;
        }


        public async Task<MessageForSelect> DeleteMessage(int messageId)
        {
            var deletionResult = await _httpClient.DeleteAsync($"Msg/Messages/{messageId}");
            var deletionContent = await deletionResult.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<MessageForSelect>(deletionContent, _options);
            return result;
        }

        public async Task MarkMessagesRead(string receiverEmail, string senderEmail)
        {
            var registrationResult = await _httpClient.PostAsync($"Msg/Messages/MarkRead?receiverEmail={receiverEmail}&senderEmail={senderEmail}", null);
            var registrationContent = await registrationResult.Content.ReadAsStringAsync();
        }
    }
}
