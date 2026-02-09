using Microsoft.AspNetCore.Mvc;
using ToolsWebsite.Services;
using ToolsWebsite.Models;

namespace ToolsWebsite.Controllers
{
    public class FcmController(FcmService fcmService, OneSignalService oneSignalService) : Controller
    {
        private readonly FcmService _fcmService = fcmService;
        private readonly OneSignalService _oneSignalService = oneSignalService;

        public IActionResult Index()
        {
            var model = new FcmTestModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(FcmTestModel model)
        {
            // Provider-specific validation
            if (model.Provider == NotificationProvider.Fcm)
            {
                if (string.IsNullOrWhiteSpace(model.ServerKey))
                    ModelState.AddModelError(nameof(model.ServerKey), "Service Account JSON is required for FCM.");
                if (string.IsNullOrWhiteSpace(model.DeviceToken))
                    ModelState.AddModelError(nameof(model.DeviceToken), "Device Token is required for FCM.");
            }
            else if (model.Provider == NotificationProvider.OneSignal)
            {
                if (string.IsNullOrWhiteSpace(model.OneSignalAppId))
                    ModelState.AddModelError(nameof(model.OneSignalAppId), "App ID is required for OneSignal.");
                if (string.IsNullOrWhiteSpace(model.OneSignalApiKey))
                    ModelState.AddModelError(nameof(model.OneSignalApiKey), "REST API Key is required for OneSignal.");
                if (string.IsNullOrWhiteSpace(model.OneSignalUserIds) && string.IsNullOrWhiteSpace(model.OneSignalSegments))
                    ModelState.AddModelError(nameof(model.OneSignalUserIds), "Provide either User IDs or Segments.");
            }

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var result = model.Provider switch
                {
                    NotificationProvider.OneSignal => await _oneSignalService.SendNotificationAsync(model),
                    _ => await _fcmService.SendNotificationAsync(model)
                };

                model.Result = result.Success
                    ? $"Notification sent successfully!{(result.Response != null ? $"\n{result.Response}" : "")}"
                    : $"Error: {result.ErrorMessage}";
                model.IsSuccess = result.Success;
            }
            catch (Exception ex)
            {
                model.Result = $"Exception: {ex.Message}";
                model.IsSuccess = false;
            }

            return View("Index", model);
        }
    }
}