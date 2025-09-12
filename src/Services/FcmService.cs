using System.Text;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using ToolsWebsite.Models;

namespace ToolsWebsite.Services
{
    public class FcmService(HttpClient httpClient)
    {
        private readonly HttpClient _httpClient = httpClient;

        // Sends an FCM message using the HTTP v1 API (OAuth2) instead of the legacy server key API.
        // Paste a Google Service Account JSON into the ServerKey field (must include project_id and private key).
        public async Task<FcmResult> SendNotificationAsync(FcmTestModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.DeviceToken))
                {
                    return new FcmResult { Success = false, ErrorMessage = "Device token is required." };
                }

                if (string.IsNullOrWhiteSpace(model.ServerKey) || !model.ServerKey.TrimStart().StartsWith("{"))
                {
                    return new FcmResult
                    {
                        Success = false,
                        ErrorMessage = "Provide Google Service Account JSON in the 'Server Key' field to use FCM HTTP v1."
                    };
                }

                // Extract project_id from the service account JSON
                string projectId;
                try
                {
                    using var doc = JsonDocument.Parse(model.ServerKey);
                    if (!doc.RootElement.TryGetProperty("project_id", out var projEl) || string.IsNullOrWhiteSpace(projEl.GetString()))
                    {
                        return new FcmResult { Success = false, ErrorMessage = "Service account JSON missing project_id." };
                    }
                    projectId = projEl.GetString()!;
                }
                catch (Exception ex)
                {
                    return new FcmResult { Success = false, ErrorMessage = $"Invalid service account JSON: {ex.Message}" };
                }

                // Initialize Google OAuth2 credential and get access token
                GoogleCredential credential;
                try
                {
                    credential = GoogleCredential.FromJson(model.ServerKey)
                        .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
                }
                catch (Exception ex)
                {
                    return new FcmResult { Success = false, ErrorMessage = $"Failed to initialize Google credentials: {ex.Message}" };
                }

                string accessToken;
                try
                {
                    accessToken = await ((ITokenAccess)credential).GetAccessTokenForRequestAsync();
                }
                catch (Exception ex)
                {
                    return new FcmResult { Success = false, ErrorMessage = $"Failed to obtain access token: {ex.Message}" };
                }

                var url = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";

                // Parse optional data JSON
                object? dataPayload = null;
                if (!string.IsNullOrWhiteSpace(model.Data))
                {
                    try
                    {
                        // Keep as JsonElement so it serializes inline
                        dataPayload = JsonSerializer.Deserialize<JsonElement>(model.Data);
                    }
                    catch (Exception ex)
                    {
                        return new FcmResult { Success = false, ErrorMessage = $"Invalid Data JSON: {ex.Message}" };
                    }
                }

                // Build message payload using dictionaries to preserve underscore field names where required
                var message = new Dictionary<string, object?>
                {
                    ["token"] = model.DeviceToken,
                    ["notification"] = new Dictionary<string, object?>
                    {
                        ["title"] = model.Title,
                        ["body"] = model.Body,
                        ["image"] = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl
                    },
                    ["data"] = dataPayload
                };

                var payload = new Dictionary<string, object?>
                {
                    ["message"] = message
                };

                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.SendAsync(request);
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