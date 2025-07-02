using InsurancePolicy.Domain.Entities;
using InsurancePolicy.Domain.Enums;
using MongoDB.Driver;

namespace InsurancePolicy.Repository.CarInfos;

public interface ICarRepository
{
    Task Add(CarEntity command, CancellationToken cancellationToken);
    Task<CarDocument?> GetByBrandAndModel(CarBrand brand, string model, int carModelYear, CancellationToken cancellationToken);
}

public class CarRepository : ICarRepository
{
    private readonly IMongoCollection<CarDocument> _collection;

    public CarRepository(DatabaseSettings settings)
    {
        var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);

        _collection = database.GetCollection<CarDocument>("cars");
    }

    public async Task Add(CarEntity command, CancellationToken cancelToken)
    {
        await _collection.InsertOneAsync(CarDocument.FromEntity(command), cancellationToken: cancelToken);

        return;
    }

    public async Task<CarDocument?> GetByBrandAndModel(CarBrand brand, string model, int carModelYear, CancellationToken cancellationToken)
    {
        var entity = await _collection
            .Find(x => x.BrandId == (int)brand && x.ModelName == model && x.Year == carModelYear)
            .FirstOrDefaultAsync(cancellationToken);

        return entity;
    }
}