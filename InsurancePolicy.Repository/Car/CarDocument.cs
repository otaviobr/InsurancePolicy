namespace InsurancePolicy.Repository.CarInfos;

using InsurancePolicy.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CarDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public decimal Price { get; set; }
    public int ModelId { get; set; }
    public string ModelName { get; set; }
    public int BrandId { get; set; }
    public string BrandName { get; set; }
    public int Year { get; set; }
    public int FuelType { get; set; }

    public static CarEntity ToEntity(CarDocument doc)
    {
        return new CarEntity
        {
            Id = doc.Id,
            Price = doc.Price,
            ModelId = doc.ModelId,
            ModelName = doc.ModelName,
            BrandId = doc.BrandId,
            BrandName = doc.BrandName,
            Year = doc.Year,
            FuelType = doc.FuelType
        };
    }

    public static CarDocument FromEntity(CarEntity entity)
    {
        return new CarDocument
        {
            Id = entity.Id,
            Price = entity.Price,
            ModelId = entity.ModelId,
            ModelName = entity.ModelName,
            BrandId = entity.BrandId,
            BrandName = entity.BrandName,
            Year = entity.Year,
            FuelType = entity.FuelType
        };
    }
}

