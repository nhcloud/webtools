using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ToolsWebsite.Models;

namespace ToolsWebsite.Controllers;

public class JsonFormatterController : Controller
{
    [HttpGet("json")] 
    public IActionResult Index()
    {
        var model = new JsonFormatterModel();
        ViewData["Title"] = "JSON Formatter & Validator";
        ViewData["Description"] = "Free JSON formatter and validator tool. Format (pretty-print) or minify JSON instantly. Validate JSON syntax online with error highlighting.";
        return View(model);
    }

    [HttpPost("json/format")] 
    [ValidateAntiForgeryToken]
    public IActionResult Format(JsonFormatterModel model)
    {
        ViewData["Title"] = "JSON Formatter & Validator";
        ViewData["Description"] = "Free JSON formatter and validator tool. Format (pretty-print) or minify JSON instantly. Validate JSON syntax online with error highlighting.";

        if (string.IsNullOrWhiteSpace(model.InputJson))
        {
            model.IsSuccess = false;
            model.ErrorMessage = "Please enter JSON to format.";
            return View("Index", model);
        }

        try
        {
            using var doc = JsonDocument.Parse(model.InputJson);
            if (model.Minify)
            {
                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
                {
                    doc.RootElement.WriteTo(writer);
                }
                model.FormattedJson = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }
            else
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                model.FormattedJson = JsonSerializer.Serialize(doc.RootElement, options);
            }
            model.IsSuccess = true;
        }
        catch (JsonException ex)
        {
            model.IsSuccess = false;
            model.ErrorMessage = $"Invalid JSON: {ex.Message}";
        }
        catch (Exception ex)
        {
            model.IsSuccess = false;
            model.ErrorMessage = $"Unexpected error: {ex.Message}";
        }

        return View("Index", model);
    }
}
