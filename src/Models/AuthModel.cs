using System.ComponentModel.DataAnnotations;

namespace ToolsWebsite.Models
{
    public class AuthModel
    {
        [Required]
        [Display(Name = "Tenant ID")]
        public string TenantId { get; set; } = "";

        [Required]
        [Display(Name = "Client ID")]
        public string ClientId { get; set; } = "";

        [Required]
        [Display(Name = "Client Secret")]
        public string ClientSecret { get; set; } = "";

        [Display(Name = "Scope")]
        public string Scope { get; set; } = "https://graph.microsoft.com/.default";

        public string? AccessToken { get; set; }
        public int? ExpiresIn { get; set; }
        public string? TokenType { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class TokenResult
    {
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
        public int? ExpiresIn { get; set; }
        public string? TokenType { get; set; }
        public string? ErrorMessage { get; set; }
    }
}