using ToolsWebsite.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<AuthService>();
builder.Services.AddHttpClient<FcmService>();
builder.Services.AddScoped<QrCodeService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

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
    defaults: new { controller = "Fcm" });

app.MapControllerRoute(
    name: "qr",
    pattern: "qr/{action=Index}",
    defaults: new { controller = "QrCode" });

app.MapControllerRoute(
    name: "auth",
    pattern: "auth/{action=Index}",
    defaults: new { controller = "Auth" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
