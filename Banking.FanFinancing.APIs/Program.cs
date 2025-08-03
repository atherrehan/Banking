using Banking.FanFinancing.Shared.Models;
using Microsoft.OpenApi.Models;
using Banking.FanFinancing.Shared.Middleware;
using Banking.FanFinancing.Shared.Configs;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Banking.FanFinancing.Infrastructure;
using Banking.FanFinancing.Domain;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});
//builder.Host.UseSerilog((ctx, lc) => lc
//.WriteTo.Console()
//.WriteTo.File("/root/Banking/Logs/myapp-log.txt", rollingInterval: RollingInterval.Day));
LogConfigs.ConfigureSerilog(builder.Configuration);
builder.Host.UseSerilog(); 
var services = builder.Services;

LogConfigs.ConfigureSerilog(builder.Configuration);

services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

});
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("IPPolicy", context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,                               
            Window = TimeSpan.FromMinutes(1),              
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
builder.Services.AddEndpointsApiExplorer();
services.Configure<DatabaseConnection>(builder.Configuration.GetSection("ConnectionStrings"));
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddHttpClient("client", client =>
{
    client.Timeout = TimeSpan.FromSeconds(90);
})
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator//,
            //UseProxy = true,
            //UseDefaultCredentials = true
            //SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
        };
        return handler;
    });
InfrastuctureDependencies.AllDependencies(services, builder.Configuration);
DomainDependencies.AllDependencies(services, builder.Configuration);


var app = builder.Build();
app.UseRateLimiter();

app.UseMiddleware<ExceptionMiddleware>();

app.UseMiddleware<AuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
