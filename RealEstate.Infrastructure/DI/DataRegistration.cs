using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RealEstate.Application.Interfaces;
using RealEstate.Application.Services;
using RealEstate.Application.Validation;

using RealEstate.Domain.Interfaces;

using RealEstate.Infrastructure.EF;
using RealEstate.Infrastructure.EF.Repositories;
using RealEstate.Infrastructure.Memory.Repositories;

namespace RealEstate.Infrastructure.DI;

public static class DataRegistration
{
    /// <summary>
    /// Application core services (Service layer + Validation service)
    /// </summary>
    public static IServiceCollection AddApplicationCore(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddSingleton<IValidationService, ValidationService>();
        return services;
    }

    /// <summary>
    /// Swapable data provider (SqlServer | Memory) + bind validation options
    /// </summary>
    public static IServiceCollection AddDataProvider(this IServiceCollection services, IConfiguration cfg)
    {
        // Bind configurable validation rules from appsettings.json
        services.Configure<ValidationOptions>(cfg.GetSection("ValidationRules").Bind);

        var provider = cfg.GetValue<string>("DataProvider") ?? "Memory";

        if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));
            services.AddScoped<IProductRepository, EfProductRepository>();
        }
        else
        {
            // Default in-memory store (no database needed)
            services.AddSingleton<IProductRepository, InMemoryProductRepository>();
        }

        return services;
    }
}
