using NorthwindTraders.Frontend.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/frontend-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Frontend starting up");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    var gatewayUrl = builder.Configuration["GatewayUrl"] ?? "http://localhost:5000";

    builder.Services.AddControllersWithViews();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddHttpClient<ApiService>(client =>
    {
        client.BaseAddress = new Uri(gatewayUrl);
    });

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.IdleTimeout = TimeSpan.FromHours(1);
    });

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
        app.UseExceptionHandler("/Home/Error");

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseSession();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Frontend terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
