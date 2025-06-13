using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MaxSystemWebSite.Services
{
    public class ChatBot : ActivityHandler
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ChatBot(IConfiguration config)
        {
            _config = config;
            _httpClient = new HttpClient();
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var userMessage = turnContext.Activity.Text;
            var reply = await CallChatGPTAsync(userMessage);
            await turnContext.SendActivityAsync(MessageFactory.Text(reply), cancellationToken);
        }

        private async Task<string> CallChatGPTAsync(string prompt)
        {
            var endpoint = _config["ChatGPT:EndPoint"];
            var apiKey = _config["ChatGPT:SecretKey"];
            var model = _config["ChatGPT:Model"];
            var request = new
            {
                model = model,
                messages = new[] { new { role = "user", content = prompt } }
            };

            var httpReq = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpReq.Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            httpReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.SendAsync(httpReq);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(json);
            return result.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }

}
