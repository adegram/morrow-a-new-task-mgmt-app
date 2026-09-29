using Taskflow.Data;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);
var vercelPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(vercelPort))
    builder.WebHost.UseUrls($"http://0.0.0.0:{vercelPort}");
builder.Services.AddRazorPages();
builder.Services.AddSingleton<TaskStore>();
var databaseUrl = builder.Configuration["DATABASE_URL"] ?? builder.Configuration["POSTGRES_URL"];
if (string.IsNullOrWhiteSpace(databaseUrl))
    throw new InvalidOperationException("Set DATABASE_URL to your hosted PostgreSQL connection string.");
var connectionString = TaskStore.BuildConnectionString(databaseUrl);
builder.Services.AddDataProtection()
    .SetApplicationName("Morrow")
    .AddKeyManagementOptions(options => options.XmlRepository = new PostgresXmlRepository(connectionString));

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
