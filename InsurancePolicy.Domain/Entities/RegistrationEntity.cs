using InsurancePolicy.Domain.Enums;
using System.Text.Json.Serialization;

namespace InsurancePolicy.Domain.Entities;

public class RegistrationEntity
{
    public string PolicyId { get; set; } = Guid.NewGuid().ToString("N");
    public InsurancePolicyStatus Status { get; set; }
    public string InsuredName { get; set; }
    public DateOnly InsuredDateOfBirth { get; set; }
    public decimal PremiumAmount { get; set; }
    public decimal CoverageAmount { get; set; }
    public string BrokerName { get; set; }

    public DateOnly CoverageStartDate { get; set; }
    public DateOnly CoverageEndDate { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime ValidUntil { get; set; }

    public decimal CarPrice { get; set; }

    [JsonIgnore]
    public string CarModelId { get; set; }

    public string CarModel { get; set; }

    [JsonIgnore]
    public CarBrand CarBrandId { get; set; }

    public string CarBrand { get; set; }

    public int CarModelYear { get; set; }

    public bool IsValidToActivation() => DateTime.UtcNow.Date <= ValidUntil.Date;

    public bool IsUpdatable() =>
        Status != InsurancePolicyStatus.Active
        && Status != InsurancePolicyStatus.Cancelled
        && Status != InsurancePolicyStatus.Rejected
        && Status != InsurancePolicyStatus.Expired;
}
