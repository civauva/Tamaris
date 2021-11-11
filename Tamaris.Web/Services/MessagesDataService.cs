﻿using System.Net.Http.Json;
using System.Text.Json;
using Tamaris.Domains.Msg;

namespace Tamaris.Web.Services
{
    public class MessagesDataService : IMessagesDataService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public MessagesDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IEnumerable<MessageForChat>> GetAllMessagesForChat()
        {
            return await JsonSerializer.DeserializeAsync<IEnumerable<MessageForChat>>
                (await _httpClient.GetStreamAsync($"Msg/Messages/ForChat"), _options);
        }


        public async Task<IEnumerable<MessageForChat>> GetMessagesForChatBetween(string username1, string username2)
        {
            return await JsonSerializer.DeserializeAsync<IEnumerable<MessageForChat>>
                   (await _httpClient.GetStreamAsync($"Msg/Messages/Conversation/{username1}/{username2}"), _options);
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

    }
}