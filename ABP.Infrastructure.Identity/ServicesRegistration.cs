using ABP.Infrastructure.Identity.Contexts;
using ABP.Infrastructure.Identity.Entities;
using ABP.Infrastructure.Identity.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ABP.Infrastructure.Identity
{
    public static class ServicesRegistration
    {
        public static void AddIdentityLayerIocForWebApp(this IServiceCollection services, IConfiguration config)
        {
            GeneralConfiguration(services, config);

            #region Identity

            services.Configure<IdentityOptions>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = false;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = true;

                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.AllowedForNewUsers = true;

                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
            });

            services.AddIdentityCore<AppUser>()
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(12);
            });

            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = IdentityConstants.ApplicationScheme;
                opt.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                opt.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
            }).AddCookie(IdentityConstants.ApplicationScheme, opt =>
            {
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(180);
                opt.SlidingExpiration = true;
                opt.LoginPath = "/Login";
                opt.AccessDeniedPath = "/Login/AccessDenied";
            });
            #endregion
        }

        public static async Task RunIdentitySeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await DefaultUserRoles.SeedAsync(roleManager);
            await DefaultAdminUser.SeedAsync(userManager);
            await DefaultCashierUser.SeedAsync(userManager);
            await DefaultClientUser.SeedAsync(userManager);
        }

        private static void GeneralConfiguration(IServiceCollection services, IConfiguration config)
        {
            #region Contexts
            if (config.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<IdentityContext>(opt =>
                opt.UseInMemoryDatabase("ArtemisBankingAppDb"));
            }
            else
            {
                var connectionString = config.GetConnectionString("IdentityConnection");
                services.AddDbContext<IdentityContext>(

                    (serviceProvider, opt) =>
                    {
                        opt.UseSqlServer(connectionString,
                        m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName));
                    },
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Scoped
                    );
            }
            #endregion
        }
    }
}
