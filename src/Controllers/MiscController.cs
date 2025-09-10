using Microsoft.AspNetCore.Mvc;

namespace ToolsWebsite.Controllers;

public class MiscController : Controller
{
    [HttpGet("misc")] // friendly short path
    public IActionResult Index()
    {
        ViewData["Title"] = "Miscellaneous Developer Tools";
        ViewData["Description"] = "Free miscellaneous developer tools: UNIX time converter (UTC/local), URL encode/decode, Base64 encode/decode.";
        return View();
    }
}
