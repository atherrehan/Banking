using Banking.FanFinancing.Infrastructure.Repository;
using Banking.FanFinancing.Infrastructure.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Services.Interface;
using Banking.FanFinancing.Shared.Services;
using Banking.FanFinancing.Shared.DbContext;
using Banking.FanFinancing.Shared.HttpClient.Interface;
using Banking.FanFinancing.Shared.HttpClient;
using Banking.FanFinancing.Shared.Repositry.Interfaces;
using Banking.FanFinancing.Shared.Repositry;


namespace Banking.FanFinancing.Infrastructure
{
    public static class InfrastuctureDependencies
    {
        public static IServiceCollection AllDependencies(this IServiceCollection _service, IConfiguration _configuration)
        {
            _service.Configure<JwtSettings>(_configuration.GetSection("JwtConfigurations"));
            _service.Configure<DatabaseConnection>(_configuration.GetSection("ConnectionStrings"));
            _service.Configure<CacheTimeConfig>(_configuration.GetSection("CacheTimeConfig"));
            _service.AddScoped<IDbContext, DbContext>();
            _service.AddScoped<IHTTPClientFactoryService, HTTPClientFactoryService>();
            _service.AddScoped<IAuthTokenService, AuthTokenService>();
            _service.AddScoped<ICacheService, CacheService>();
            _service.AddScoped<ICustomerRepository, CustomerRepository>();
            _service.AddScoped<IGuidService, GuidService>();
            _service.AddScoped<ILoggerService, LoggerService>();
            _service.AddDistributedMemoryCache();
            _service.AddHostedService<BackgroundTaskService>();
            _service.AddScoped<ISharedRepository, SharedRepository>();
            _service.AddSingleton<IBackgroundTaskQueue>(ctx =>
            {
                if (!int.TryParse(_configuration["QueueCapacity"], out var queueCapacity))
                    queueCapacity = 10000;
                return new BackgroundTaskQueue(queueCapacity);
            });
            return _service;
        }
    }
}
