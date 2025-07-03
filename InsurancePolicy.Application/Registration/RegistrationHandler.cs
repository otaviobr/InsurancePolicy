using InsurancePolicy.Domain.Entities;
using InsurancePolicy.Domain.Enums;
using InsurancePolicy.Repository.CarInfos;
using InsurancePolicy.Repository.Registration;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;

namespace InsurancePolicy.Application.Registration;

public interface IRegistrationHandler
{
    Task<RegistrationEntity?> Handle(RegistrationCommand command, CancellationToken cancellationToken);
}

public class RegistrationHandler : IRegistrationHandler
{
    private readonly IRegistrationRepository _repository;
    private readonly ICarRepository _carRepository;

    public RegistrationHandler(IRegistrationRepository repository, ICarRepository carRepository)
    {
        _repository = repository;
        _carRepository = carRepository;
    }

    public async Task<RegistrationEntity?> Handle(RegistrationCommand command, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetByBrandAndModel(command.CarBrandId, command.CarModel, command.CarModelYear, cancellationToken);

        if (car == null)
            return default;

        var now = DateTime.UtcNow;

        var policyValue = car.Price * 0.01m;
        var premiumAmount = policyValue;

        if (now.Year - car.Year > 10)
            premiumAmount += policyValue * 0.15m;

        if (car.Price > 100000m)
            premiumAmount += policyValue * 0.10m;

        var insuredLessThan25YearsOld = (now.Year - command.InsuredDateOfBirth.Year) < 25;
        if (insuredLessThan25YearsOld)
            premiumAmount += policyValue * 0.20m;

        var insuredGreaterThan60YearsOld = (now.Year - command.InsuredDateOfBirth.Year) > 60;
        if (insuredGreaterThan60YearsOld)
            premiumAmount += policyValue * 0.30m;

        var isurancePolicy = new RegistrationEntity
        {
            InsuredName = command.InsuredName,
            PremiumAmount = Math.Round(premiumAmount, 2),
            CoverageAmount = Math.Round(car.Price, 2),
            BrokerName = command.BrokerName,
            InsuredDateOfBirth = command.InsuredDateOfBirth,
            CarPrice = car.Price,
            CarModelId = command.CarModel,
            CarModel = command.CarModel,
            CarBrandId = command.CarBrandId,
            CarBrand = command.CarBrand.ToString(),
            CarModelYear = command.CarModelYear,
            CoverageStartDate = DateOnly.FromDateTime(now.Date),
            CoverageEndDate = DateOnly.FromDateTime(now.Date.AddYears(1)),
            Status = InsurancePolicyStatus.Pending,
            ValidUntil = now.Date.AddDays(7)
        };

        await _repository.Add(isurancePolicy, cancellationToken);

        return isurancePolicy;
    }
}
