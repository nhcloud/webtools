using System.ComponentModel.DataAnnotations;

namespace ToolsWebsite.Models
{
    public class FcmTestModel
    {
        [Required]
        [Display(Name = "Server Key")]
        public string ServerKey { get; set; } = "";

        [Required]
        [Display(Name = "Device Token")]
        public string DeviceToken { get; set; } = "";

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; } = "";

        [Required]
        [Display(Name = "Body")]
        public string Body { get; set; } = "";

        [Display(Name = "Data (JSON)")]
        public string? Data { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        public string? Result { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class FcmResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Response { get; set; }
    }
}