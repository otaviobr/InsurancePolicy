using InsurancePolicy.Domain.Entities;
using InsurancePolicy.Domain.Enums;
using MongoDB.Driver;

namespace InsurancePolicy.Repository.Registration;

public interface IRegistrationRepository
{
    Task Add(RegistrationEntity command, CancellationToken cancellationToken);
    Task<RegistrationEntity> GetByPolicyId(string policyId, CancellationToken cancellationToken);
    Task<RegistrationEntity> UpdateByPolicyId(string policyId, InsurancePolicyStatus status, CancellationToken cancellationToken);
}

public class RegistrationRepository : IRegistrationRepository
{
    private readonly IMongoCollection<RegistrationDocument> _collection;

    public RegistrationRepository(DatabaseSettings settings)
    {
        var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
        
        _collection = database.GetCollection<RegistrationDocument>("registrations");
    }

    public async Task Add(RegistrationEntity command, CancellationToken cancelToken)
    {
        await _collection.InsertOneAsync(RegistrationDocument.FromEntity(command), cancellationToken: cancelToken);

        return;
    }

    public async Task<RegistrationEntity> GetByPolicyId(string policyId, CancellationToken cancellationToken)
    {
        var document = await _collection.Find(x => x.PolicyId == policyId).FirstAsync(cancellationToken);

        return RegistrationDocument.ToEntity(document);
    }
    
    public async Task<RegistrationEntity> UpdateByPolicyId(string policyId, InsurancePolicyStatus status, CancellationToken cancellationToken)
    {
        var updateDefinition = new UpdateDefinitionBuilder<RegistrationDocument>()
            .Set(x => x.InsurancePolicyStatus, status)
            .Set(x => x.IssueDate, DateTime.UtcNow)
            .Set(x => x.ValidUntil, DateTime.MinValue);

        var findOptions = new FindOneAndUpdateOptions<RegistrationDocument>()
        {
            ReturnDocument = ReturnDocument.After
        };

        var document = await _collection.FindOneAndUpdateAsync(x => x.PolicyId == policyId, updateDefinition, findOptions, cancellationToken: cancellationToken);

        return RegistrationDocument.ToEntity(document);
    }
}
