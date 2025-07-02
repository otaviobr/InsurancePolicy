using InsurancePolicy.Domain.Entities;
using InsurancePolicy.Domain.Enums;
using InsurancePolicy.Repository.Registration;

namespace InsurancePolicy.Application.Activation;

public interface IUpdatePolicyStatusHandler
{
    Task<RegistrationEntity?> Handle(string policyId, InsurancePolicyStatus status, CancellationToken cancellationToken);
}

public class UpdatePolicyStatusHandler : IUpdatePolicyStatusHandler
{
    private readonly IRegistrationRepository _repository;

    public UpdatePolicyStatusHandler(IRegistrationRepository repository)
    {
        _repository = repository;
    }

    public async Task<RegistrationEntity?> Handle(string policyId, InsurancePolicyStatus status, CancellationToken cancellationToken)
    {
        var policy = await _repository.GetByPolicyId(policyId, cancellationToken);

        if (policy.IsUpdatable() && policy.IsValidToActivation())
        {
            var policyActivated = await _repository.UpdateByPolicyId(policyId, status, cancellationToken);

            return policyActivated;
        }

        return policy;
    }
}
