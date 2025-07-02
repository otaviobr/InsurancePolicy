using InsurancePolicy.Application.Activation;
using InsurancePolicy.Application.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace InsurancePolicy.CrossServiceRegister;

public static class AddApplicationService
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IRegistrationHandler, RegistrationHandler>();
        services.AddScoped<IUpdatePolicyStatusHandler, UpdatePolicyStatusHandler>();

        return services;
    }
}
