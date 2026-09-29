using Microsoft.AspNetCore.Authentication.Cookies;
using Taskflow.Data;

var builder = WebApplication.CreateBuilder(args);
var vercelPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(vercelPort))
    builder.WebHost.UseUrls($"http://0.0.0.0:{vercelPort}");
builder.Services.AddRazorPages();
builder.Services.AddSingleton<TaskStore>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.Cookie.Name = "morrow.auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

if (builder.Environment.IsProduction() && string.IsNullOrWhiteSpace(builder.Configuration["APP_PASSWORD"]))
    throw new InvalidOperationException("Set APP_PASSWORD in the deployment environment before starting Morrow.");

var app = builder.Build();
var store = app.Services.GetRequiredService<TaskStore>();
await store.InitializeAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
