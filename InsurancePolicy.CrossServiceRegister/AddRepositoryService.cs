using InsurancePolicy.Repository;
using InsurancePolicy.Repository.CarInfos;
using InsurancePolicy.Repository.Registration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InsurancePolicy.CrossServiceRegister;

public static class AddRepositoryService
{
    public static IServiceCollection AddRepositoryServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (!configuration.GetSection(nameof(DatabaseSettings)).Exists())
            throw new ArgumentNullException(nameof(configuration), $"{nameof(DatabaseSettings)} is missing in configuration.");

        var dbSettings = configuration.GetSection(nameof(DatabaseSettings));

        var dbName = dbSettings.GetRequiredSection(nameof(DatabaseSettings.DatabaseName)).Value;

        var connStr = dbSettings.GetRequiredSection(nameof(DatabaseSettings.ConnectionString)).Value;

        services.AddSingleton(serviceProvider =>
        {
            return new DatabaseSettings
            {
                DatabaseName = dbName ?? "",
                ConnectionString = connStr ?? ""
            };
        });

        services.AddScoped<IRegistrationRepository, RegistrationRepository>();
        services.AddScoped<ICarRepository, CarRepository>();

        return services;
    }
}
