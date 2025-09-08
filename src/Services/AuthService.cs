using System.Text.Json;
using ToolsWebsite.Models;

namespace ToolsWebsite.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<TokenResult> GetAccessTokenAsync(AuthModel model)
        {
            try
            {
                var tokenUrl = $"https://login.microsoftonline.com/{model.TenantId}/oauth2/v2.0/token";

                var requestBody = new List<KeyValuePair<string, string>>
                {
                    new("client_id", model.ClientId),
                    new("client_secret", model.ClientSecret),
                    new("scope", model.Scope),
                    new("grant_type", "client_credentials")
                };

                var content = new FormUrlEncodedContent(requestBody);
                var response = await _httpClient.PostAsync(tokenUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    return new TokenResult
                    {
                        Success = true,
                        AccessToken = tokenResponse.GetProperty("access_token").GetString(),
                        ExpiresIn = tokenResponse.GetProperty("expires_in").GetInt32(),
                        TokenType = tokenResponse.GetProperty("token_type").GetString()
                    };
                }
                else
                {
                    return new TokenResult
                    {
                        Success = false,
                        ErrorMessage = $"HTTP {response.StatusCode}: {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new TokenResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}