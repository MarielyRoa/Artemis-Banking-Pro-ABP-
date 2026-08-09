using Microsoft.Extensions.DependencyInjection;

namespace ABP.Core.Application
{
    public static class ServicesRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Register application services
            services.AddScoped<ICashierService, CashierService>();
        }
    }
}
