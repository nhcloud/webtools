using System.ComponentModel.DataAnnotations;

namespace ToolsWebsite.Models
{
    public enum NotificationProvider
    {
        [Display(Name = "FCM (Firebase Cloud Messaging)")]
        Fcm,
        [Display(Name = "OneSignal")]
        OneSignal
    }

    public class FcmTestModel
    {
        [Display(Name = "Provider")]
        public NotificationProvider Provider { get; set; } = NotificationProvider.Fcm;

        // --- FCM fields ---
        [Display(Name = "Service Account JSON")]
        public string ServerKey { get; set; } = "";

        [Display(Name = "Device Token")]
        public string DeviceToken { get; set; } = "";

        // --- OneSignal fields ---
        [Display(Name = "App ID")]
        public string? OneSignalAppId { get; set; }

        [Display(Name = "REST API Key")]
        public string? OneSignalApiKey { get; set; }

        [Display(Name = "User IDs (comma-separated)")]
        public string? OneSignalUserIds { get; set; }

        [Display(Name = "Segments (comma-separated)")]
        public string? OneSignalSegments { get; set; }

        // --- Shared fields ---
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