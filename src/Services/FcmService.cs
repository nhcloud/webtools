using System.Text;
using System.Text.Json;
using ToolsWebsite.Models;

namespace ToolsWebsite.Services
{
    public class FcmService
    {
        private readonly HttpClient _httpClient;
        private const string FCM_URL = "https://fcm.googleapis.com/fcm/send";

        public FcmService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FcmResult> SendNotificationAsync(FcmTestModel model)
        {
            try
            {
                var payload = new
                {
                    to = model.DeviceToken,
                    notification = new
                    {
                        title = model.Title,
                        body = model.Body,
                        image = model.ImageUrl,
                        click_action = model.ClickAction
                    },
                    data = !string.IsNullOrEmpty(model.Data) ? JsonSerializer.Deserialize<object>(model.Data) : null
                };

                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"key={model.ServerKey}");

                var response = await _httpClient.PostAsync(FCM_URL, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new FcmResult
                    {
                        Success = true,
                        Response = responseContent
                    };
                }
                else
                {
                    return new FcmResult
                    {
                        Success = false,
                        ErrorMessage = $"HTTP {response.StatusCode}: {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new FcmResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}