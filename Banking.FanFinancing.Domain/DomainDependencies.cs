using Banking.FanFinancing.Domain.Services;
using Banking.FanFinancing.Domain.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Banking.FanFinancing.Domain
{
    public static class DomainDependencies
    {
        public static void AllDependencies(IServiceCollection _service, IConfiguration _configuration)
        {
            _service.AddScoped<IAccountService, AccountService>();
            _service.AddScoped<ICustomerService, CustomerService>();
            _service.AddScoped<ILoanService, LoanService>();
        }
    }
}
