using System.ComponentModel.DataAnnotations;

namespace ToolsWebsite.Models;

public class JsonFormatterModel
{
    [Display(Name = "Input JSON")] 
    public string? InputJson { get; set; }

    [Display(Name = "Formatted JSON")] 
    public string? FormattedJson { get; set; }

    public bool Minify { get; set; }

    public bool IsSuccess { get; set; }

    public string? ErrorMessage { get; set; }
}
