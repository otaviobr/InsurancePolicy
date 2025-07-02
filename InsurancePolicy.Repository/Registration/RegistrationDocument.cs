using InsurancePolicy.Domain.Entities;
using InsurancePolicy.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InsurancePolicy.Repository.Registration;

public class RegistrationDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string PolicyId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public InsurancePolicyStatus InsurancePolicyStatus { get; set; }

    public string InsuredName { get; set; }

    public DateTime InsuredDateOfBirth { get; set; }

    public decimal PremiumAmount { get; set; }

    public decimal CoverageAmount { get; set; }

    public string BrokerName { get; set; }

    public DateTime CoverageStartDate { get; set; }

    public DateTime CoverageEndDate { get; set; }

    public DateTime IssueDate { get; set; }
    public DateTime ValidUntil { get; set; }

    public decimal CarPrice { get; set; }

    public string CarModelId { get; set; }

    public string CarModel { get; set; }

    [BsonRepresentation(BsonType.String)]
    public CarBrand CarBrandId { get; set; }

    public string CarBrand { get; set; }

    public int CarModelYear { get; set; }

    public static RegistrationEntity ToEntity(RegistrationDocument doc)
    {
        return new RegistrationEntity
        {
            PolicyId = doc.PolicyId,
            Status = doc.InsurancePolicyStatus,
            InsuredName = doc.InsuredName,
            InsuredDateOfBirth = DateOnly.FromDateTime(doc.InsuredDateOfBirth),
            PremiumAmount = doc.PremiumAmount,
            CoverageAmount = doc.CoverageAmount,
            BrokerName = doc.BrokerName,
            CoverageStartDate = DateOnly.FromDateTime(doc.CoverageStartDate),
            CoverageEndDate = DateOnly.FromDateTime(doc.CoverageEndDate),
            IssueDate = doc.IssueDate,
            CarPrice = doc.CarPrice,
            CarModelId = doc.CarModelId,
            CarModel = doc.CarModel,
            CarBrandId = doc.CarBrandId,
            CarBrand = doc.CarBrand,
            CarModelYear = doc.CarModelYear,
            ValidUntil = doc.ValidUntil
        };
    }

    public static RegistrationDocument FromEntity(RegistrationEntity entity)
    {
        return new RegistrationDocument
        {
            PolicyId = entity.PolicyId,
            InsurancePolicyStatus = entity.Status,
            InsuredName = entity.InsuredName,
            InsuredDateOfBirth = entity.InsuredDateOfBirth.ToDateTime(TimeOnly.MinValue),
            PremiumAmount = entity.PremiumAmount,
            CoverageAmount = entity.CoverageAmount,
            BrokerName = entity.BrokerName,
            CoverageStartDate = entity.CoverageStartDate.ToDateTime(TimeOnly.MinValue),
            CoverageEndDate = entity.CoverageEndDate.ToDateTime(TimeOnly.MinValue),
            IssueDate = entity.IssueDate,
            CarPrice = entity.CarPrice,
            CarModelId = entity.CarModelId,
            CarModel = entity.CarModel,
            CarBrandId = entity.CarBrandId,
            CarBrand = entity.CarBrand,
            CarModelYear = entity.CarModelYear,
            ValidUntil = entity.ValidUntil
        };
    }
}
