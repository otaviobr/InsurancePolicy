using FluentValidation;
using InsurancePolicy.Api.Controllers.Registration;
using InsurancePolicy.CrossServiceRegister;
using System.Text.Json.Serialization;

namespace InsurancePolicy.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IValidator<SimulationRequest>, RegistrationRequestValidator>();
        builder.Services.AddApplicationServices();
        builder.Services.AddRepositoryServices(builder.Configuration);

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.Production.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
