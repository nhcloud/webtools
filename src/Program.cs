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
