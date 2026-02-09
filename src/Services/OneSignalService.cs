using System.Text;
using System.Text.Json;
using ToolsWebsite.Models;

namespace ToolsWebsite.Services
{
    public class OneSignalService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;
        private const string OneSignalApiUrl = "https://onesignal.com/api/v1/notifications";

        public async Task<FcmResult> SendNotificationAsync(FcmTestModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.OneSignalAppId))
                    return new FcmResult { Success = false, ErrorMessage = "OneSignal App ID is required." };

                if (string.IsNullOrWhiteSpace(model.OneSignalApiKey))
                    return new FcmResult { Success = false, ErrorMessage = "OneSignal REST API Key is required." };

                var hasUserIds = !string.IsNullOrWhiteSpace(model.OneSignalUserIds);
                var hasSegments = !string.IsNullOrWhiteSpace(model.OneSignalSegments);

                if (!hasUserIds && !hasSegments)
                    return new FcmResult { Success = false, ErrorMessage = "Provide either User IDs or Segments to target." };

                // Build payload
                var payload = new Dictionary<string, object>
                {
                    ["app_id"] = model.OneSignalAppId,
                    ["headings"] = new { en = model.Title },
                    ["contents"] = new { en = model.Body }
                };

                // Target: external user IDs or segments
                if (hasUserIds)
                {
                    var userIds = model.OneSignalUserIds!
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    payload["include_external_user_ids"] = userIds;
                    payload["channel_for_external_user_ids"] = "push";
                }
                else if (hasSegments)
                {
                    var segments = model.OneSignalSegments!
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    payload["included_segments"] = segments;
                }

                // Image support (Android + iOS)
                if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                {
                    payload["big_picture"] = model.ImageUrl;              // Android
                    payload["ios_attachments"] = new { id1 = model.ImageUrl }; // iOS
                    payload["mutable_content"] = true;                    // Required for iOS images
                }

                // Custom data
                if (!string.IsNullOrWhiteSpace(model.Data))
                {
                    try
                    {
                        var dataObj = JsonSerializer.Deserialize<JsonElement>(model.Data);
                        payload["data"] = dataObj;
                    }
                    catch (Exception ex)
                    {
                        return new FcmResult { Success = false, ErrorMessage = $"Invalid Data JSON: {ex.Message}" };
                    }
                }

                var json = JsonSerializer.Serialize(payload);

                using var request = new HttpRequestMessage(HttpMethod.Post, OneSignalApiUrl)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Authorization", $"Basic {model.OneSignalApiKey}");

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new FcmResult { Success = true, Response = responseContent };
                }
                else
                {
                    return new FcmResult { Success = false, ErrorMessage = $"HTTP {response.StatusCode}: {responseContent}" };
                }
            }
            catch (Exception ex)
            {
                return new FcmResult { Success = false, ErrorMessage = ex.Message };
            }
        }
    }
}
