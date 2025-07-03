using FluentValidation;
using InsurancePolicy.Application.Activation;
using InsurancePolicy.Application.Registration;
using InsurancePolicy.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePolicy.Api.Controllers.Registration;

[ApiController]
[Route("[controller]")]
public class PolicyRegistrationController : ControllerBase
{
    private readonly ILogger<PolicyRegistrationController> _logger;
    private readonly IValidator<SimulationRequest> _validator;
    private readonly IRegistrationHandler _registrationHandler;
    private readonly IUpdatePolicyStatusHandler _activationHandler;

    public PolicyRegistrationController(
        ILogger<PolicyRegistrationController> logger,
        IValidator<SimulationRequest> validator,
        IRegistrationHandler registrationHandler,
        IUpdatePolicyStatusHandler activationHandler)
    {
        _logger = logger;
        _validator = validator;
        _registrationHandler = registrationHandler;
        _activationHandler = activationHandler;
    }

    [HttpPost("Simulation")]
    public async Task<IActionResult> Simulation([FromBody] SimulationRequest request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Registration request validation failed: {Errors}", validationResult.Errors);
            return BadRequest(validationResult.ToString(";"));
        }

        if(!Enum.TryParse(typeof(CarBrand), request.CarBrand, out var carBrandResult))
            return BadRequest("Car brand not registered in the database.");

        var carBrand = (CarBrand)carBrandResult;
        var result = await _registrationHandler.Handle(new()
        {
            BrokerName = request.BrokerName,
            InsuredName = request.InsuredName,
            CarBrandId = carBrand,
            CarBrand = carBrand.ToString(),
            CarModel = request.CarModel,
            CarModelYear = request.CarModelYear,
            InsuredDateOfBirth = request.InsuredDateOfBirth
        }, cancellationToken);

        if (result is not null)
            return StatusCode(201, result);
        else
            return BadRequest("Car not registered in the database.");
    }

    [HttpPost("UpdatePolicyStatus")]
    public async Task<IActionResult> ActivatePolicy(string policyId, InsurancePolicyStatus status, CancellationToken cancellationToken)
    {
        var result = await _activationHandler.Handle(policyId, status, cancellationToken);

        if (result is not null)
            return StatusCode(200, result);
        else
            return BadRequest("Activation failed.");
    }
}

