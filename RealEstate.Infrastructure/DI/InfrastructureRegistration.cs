using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RealEstate.Domain.Interfaces;
using RealEstate.Infrastructure.EF;
using RealEstate.Infrastructure.EF.Repositories;
using RealEstate.Infrastructure.Mongo;
using RealEstate.Infrastructure.Mongo.Repositories;

namespace RealEstate.Infrastructure.DI;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var provider = cfg.GetSection("Database:Provider").Value ?? "SqlServer";

        if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<ApplicationDbContext>(opts =>
                opts.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));
            services.AddScoped<IPropertyRepository, EfPropertyRepository>();
        }
        else if (provider.Equals("Mongo", StringComparison.OrdinalIgnoreCase))
        {
            var ms = cfg.GetSection("MongoSettings").Get<MongoSettings>() ?? new();
            services.AddSingleton(ms);
            services.AddSingleton<IMongoClient>(_ => new MongoClient(ms.ConnectionString));
            services.AddScoped<IMongoDatabase>(sp => 
                sp.GetRequiredService<IMongoClient>().GetDatabase(ms.Database));
            services.AddScoped<IPropertyRepository, MongoPropertyRepository>();
        }
        else
        {
            throw new InvalidOperationException("Unknown Database Provider. Use 'SqlServer' or 'Mongo'.");
        }

        return services;
    }
}
