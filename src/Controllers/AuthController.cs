using Microsoft.AspNetCore.Mvc;
using ToolsWebsite.Services;
using ToolsWebsite.Models;

namespace ToolsWebsite.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            var model = new AuthModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateToken(AuthModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                var result = await _authService.GetAccessTokenAsync(model);
                model.AccessToken = result.AccessToken;
                model.ExpiresIn = result.ExpiresIn;
                model.TokenType = result.TokenType;
                model.IsSuccess = result.Success;
                model.ErrorMessage = result.ErrorMessage;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Exception: {ex.Message}";
                model.IsSuccess = false;
            }

            return View("Index", model);
        }
    }
}