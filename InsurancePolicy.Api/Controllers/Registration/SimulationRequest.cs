using FluentValidation;

namespace InsurancePolicy.Api.Controllers.Registration;

public record struct SimulationRequest
{
    public string InsuredName { get; set; }
    public DateOnly InsuredDateOfBirth { get; set; }
    public string BrokerName { get; set; }

    public string CarModel { get; set; }
    public string CarBrand { get; set; }
    public int CarModelYear { get; set; }
}

public class RegistrationRequestValidator : AbstractValidator<SimulationRequest>
{
    public RegistrationRequestValidator()
    {
        RuleFor(x => x.InsuredName).NotEmpty();
        RuleFor(x => x.InsuredDateOfBirth).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18))).WithMessage("The insured must be at least 18 years old.");
        RuleFor(x => x.BrokerName).NotEmpty();

        RuleFor(x => x.CarModel).NotEmpty();
        RuleFor(x => x.CarBrand).NotEmpty();
        RuleFor(x => x.CarModelYear).NotEmpty();
    }
}
