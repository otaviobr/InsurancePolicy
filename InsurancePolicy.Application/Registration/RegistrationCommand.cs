using InsurancePolicy.Domain.Enums;

namespace InsurancePolicy.Application.Registration;

public record struct RegistrationCommand
{
    public string InsuredName { get; set; }
    public DateOnly InsuredDateOfBirth { get; set; }
    public string BrokerName { get; set; }

    public string CarModel { get; set; }
    public CarBrand CarBrandId { get; set; }
    public string CarBrand { get; set; }
    public int CarModelYear { get; set; }
}
