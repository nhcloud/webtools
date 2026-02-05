using Microsoft.AspNetCore.Mvc;

namespace ToolsWebsite.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private static readonly DateTime StartTime = DateTime.UtcNow;

        [HttpGet]
        public IActionResult Get()
        {
            var uptime = DateTime.UtcNow - StartTime;

            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                uptime = new
                {
                    days = uptime.Days,
                    hours = uptime.Hours,
                    minutes = uptime.Minutes,
                    seconds = uptime.Seconds,
                    totalSeconds = (long)uptime.TotalSeconds,
                    formatted = FormatUptime(uptime)
                },
                startTime = StartTime,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                version = typeof(HealthController).Assembly.GetName().Version?.ToString() ?? "1.0.0"
            });
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpGet("live")]
        public IActionResult Live()
        {
            return Ok(new { status = "alive", timestamp = DateTime.UtcNow });
        }

        [HttpGet("ready")]
        public IActionResult Ready()
        {
            // Add any readiness checks here (database, external services, etc.)
            return Ok(new { status = "ready", timestamp = DateTime.UtcNow });
        }

        private static string FormatUptime(TimeSpan uptime)
        {
            if (uptime.TotalDays >= 1)
                return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
            if (uptime.TotalHours >= 1)
                return $"{uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
            if (uptime.TotalMinutes >= 1)
                return $"{uptime.Minutes}m {uptime.Seconds}s";
            return $"{uptime.Seconds}s";
        }
    }
}
