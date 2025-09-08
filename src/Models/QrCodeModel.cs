using System.ComponentModel.DataAnnotations;

namespace ToolsWebsite.Models
{
    public class QrCodeModel
    {
        [Required]
        [Url]
        [Display(Name = "URL to generate QR Code")]
        public string Url { get; set; } = "";

        public string? QrCodeBase64 { get; set; }
        public bool IsGenerated { get; set; }
        public string? ErrorMessage { get; set; }
    }
}