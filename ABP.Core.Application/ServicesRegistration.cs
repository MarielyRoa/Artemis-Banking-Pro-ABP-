using ABP.Core.Application.Interfaces;
using ABP.Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
using System.Reflection;

namespace ABP.Core.Application
{
    public static class ServicesRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            // AutoMapper Configuration
            services.AddAutoMapper(config =>
            {
                config.AddMaps(Assembly.GetExecutingAssembly());

            });

            services.AddTransient<IBeneficiaryService, BeneficiaryService>();
            services.AddTransient<ITransactionService, TransactionService>();
            services.AddTransient<ILoanService, LoanService>();
            services.AddTransient<ICreditCardService, CreditCardService>();
            services.AddTransient<ISavingAccountService, SavingAccountService>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        }
    }
}
