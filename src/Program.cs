using System.Threading.RateLimiting;
using ToolsWebsite.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<AuthService>();
builder.Services.AddHttpClient<FcmService>();
builder.Services.AddScoped<QrCodeService>();

// Add Rate Limiting (.NET 7+)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
    // Global rate limit per IP: 100 requests per minute
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 5
            }));
    
    // Stricter limit for auth endpoints (token generation)
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }));
    
    // Stricter limit for FCM endpoints
    options.AddPolicy("fcm", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }));

    // Handle rate limit exceeded
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        
        var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue)
            ? retryAfterValue.TotalSeconds
            : 60;
        
        context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();
        
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests",
            message = "Rate limit exceeded. Please try again later.",
            retryAfterSeconds = retryAfter
        }, cancellationToken);
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable rate limiting
app.UseRateLimiter();

// Custom middleware to handle common bot requests efficiently
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLowerInvariant();
    
    // Block common bot/scanner requests early
    if (path != null && (
        path.StartsWith("/.git") ||
        path.StartsWith("/.env") ||
        path.Contains(".env.") ||
        path.StartsWith("/config/") ||
        path.StartsWith("/admin/") ||
        path.Contains("wp-admin") ||
        path.Contains("wp-content") ||
        path.Contains("phpmyadmin")
    ))
    {
        context.Response.StatusCode = 404;
        return;
    }
    
    await next();
});

app.UseStaticFiles();
app.UseRouting();

// Map routes for each tool
app.MapControllerRoute(
    name: "fcm",
    pattern: "fcm/{action=Index}",
    defaults: new { controller = "Fcm" })
    .RequireRateLimiting("fcm");

app.MapControllerRoute(
    name: "qr",
    pattern: "qr/{action=Index}",
    defaults: new { controller = "QrCode" });

app.MapControllerRoute(
    name: "auth",
    pattern: "auth/{action=Index}",
    defaults: new { controller = "Auth" })
    .RequireRateLimiting("auth");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
