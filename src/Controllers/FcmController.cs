using Microsoft.AspNetCore.Mvc;
using ToolsWebsite.Services;
using ToolsWebsite.Models;

namespace ToolsWebsite.Controllers
{
    public class FcmController : Controller
    {
        private readonly FcmService _fcmService;

        public FcmController(FcmService fcmService)
        {
            _fcmService = fcmService;
        }

        public IActionResult Index()
        {
            var model = new FcmTestModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(FcmTestModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var result = await _fcmService.SendNotificationAsync(model);
                model.Result = result.Success ? "Notification sent successfully!" : $"Error: {result.ErrorMessage}";
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