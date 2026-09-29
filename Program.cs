using Taskflow.Data;

var builder = WebApplication.CreateBuilder(args);
var vercelPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(vercelPort))
    builder.WebHost.UseUrls($"http://0.0.0.0:{vercelPort}");
builder.Services.AddRazorPages();
builder.Services.AddSingleton<TaskStore>();

var app = builder.Build();
var store = app.Services.GetRequiredService<TaskStore>();
await store.InitializeAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/");
}
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();
