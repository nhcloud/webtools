using Microsoft.AspNetCore.Mvc;
using ToolsWebsite.Services;
using ToolsWebsite.Models;

namespace ToolsWebsite.Controllers
{
    public class QrCodeController(QrCodeService qrCodeService) : Controller
    {
        private readonly QrCodeService _qrCodeService = qrCodeService;

        public IActionResult Index()
        {
            var model = new QrCodeModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult GenerateQr(QrCodeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                model.QrCodeBase64 = _qrCodeService.GenerateQrCode(model.Url);
                model.IsGenerated = true;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Error generating QR code: {ex.Message}";
                model.IsGenerated = false;
            }

            return View("Index", model);
        }
    }
}