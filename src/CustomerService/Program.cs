using Microsoft.EntityFrameworkCore;
using NorthwindTraders.CustomerService.Data;
using NorthwindTraders.CustomerService.Repositories;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/customerservice-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("CustomerService starting up");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

    builder.Services.AddDbContext<NorthwindContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

    builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Northwind CustomerService API", Version = "v1" });
    });

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerService v1"));

    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
        });
    });

    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "CustomerService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
