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
    Task AtualizarFipe();
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

    public async Task AtualizarFipe()
    {
        var anosModelo = new List<CarInfos>();
        var limit = 20;
        var current = 0;

        var brands = Enum.GetValues(typeof(CarBrand))
            .Cast<CarBrand>()
            .Select(b => new { Id = (int)b, Name = b.ToString() })
            .ToList();

        var client = new HttpClient();

        foreach (var brand in brands)
        {
            var result = await Consulta(client, brand.Id);

            if (!result.IsSuccessStatusCode)
            {
                Console.WriteLine($"Consulta Modelo NOK: {result.IsSuccessStatusCode}");

                var tentativa = 1;
                while (!result.IsSuccessStatusCode)
                {
                    await Task.Delay(10000);
                    result = await Consulta(client, brand.Id);
                    tentativa++;
                }
                Console.WriteLine($"Consulta Modelo - tentativas: {tentativa}");
                tentativa = 0;
            }

            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                var models = JsonSerializer.Deserialize<ModelosResponse>(content);

                foreach (var model in models?.Modelos)
                {
                    var consultaAnos = await ConsultaAnoModelo(client, brand.Id, model);

                    if (!consultaAnos.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Consulta AnoModelo NOK: {consultaAnos.IsSuccessStatusCode}");
                        var tentativa = 1;
                        while (!consultaAnos.IsSuccessStatusCode)
                        {
                            await Task.Delay(10000);
                            consultaAnos = await ConsultaAnoModelo(client, brand.Id, model);
                            tentativa++;
                        }
                        Console.WriteLine($"Consulta AnoModelo - tentativas: {tentativa}");
                        tentativa = 0;
                    }

                    if (consultaAnos.IsSuccessStatusCode)
                    {
                        var contentConsultaAnos = await consultaAnos.Content.ReadAsStringAsync();
                        var valoresConsultaAnos = JsonSerializer.Deserialize<List<Ano>>(contentConsultaAnos);

                        foreach (var ano in valoresConsultaAnos)
                        {
                            var anoCombustivel = ano.Value.Split('-');

                            Console.WriteLine($"Marca: {brand.Name}; Modelo: {model.Label}; Ano: {anoCombustivel.First()}");
                            var consultaCompleta = await ConsultaCompleta(client, brand.Id, model, anoCombustivel);

                            if (!consultaCompleta.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"Consulta Completa OK: {consultaCompleta.IsSuccessStatusCode}");

                                var tentativa = 1;
                                while (!consultaCompleta.IsSuccessStatusCode)
                                {
                                    //await Task.Delay(2000 * tentativa);
                                    await Task.Delay(10000);
                                    consultaCompleta = await ConsultaCompleta(client, brand.Id, model, anoCombustivel);
                                    tentativa++;
                                }
                                Console.WriteLine($"Consulta Completa - tentativas: {tentativa}");
                                tentativa = 0;
                            }

                            if (consultaCompleta.IsSuccessStatusCode)
                            {
                                var VeiculoFipeResponseResponse = await consultaCompleta.Content.ReadAsStringAsync();
                                var VeiculoFipeResponseEntity = JsonSerializer.Deserialize<VeiculoFipeResponse>(VeiculoFipeResponseResponse);

                                await _carRepository.Add(new CarEntity()
                                {
                                    Price = decimal.Parse(VeiculoFipeResponseEntity.Valor, NumberStyles.Currency, new CultureInfo("pt-BR")),
                                    BrandId = brand.Id,
                                    BrandName = brand.Name,
                                    ModelId = model.Value,
                                    ModelName = model.Label,
                                    FuelType = int.Parse(anoCombustivel.Last()),
                                    Year = int.Parse(anoCombustivel.First())
                                }, CancellationToken.None);
                            }

                            await Task.Delay(1000);
                        }
                    }
                }
            }
        }
    }

    private static async Task<HttpResponseMessage> Consulta(HttpClient client, int brandId)
    {
        return await client.PostAsJsonAsync("https://veiculos.fipe.org.br/api/veiculos/ConsultarModelos", new { codigoTabelaReferencia = 323, codigoTipoVeiculo = 1, codigoMarca = brandId });
    }

    private static async Task<HttpResponseMessage> ConsultaAnoModelo(HttpClient client, int brandId, Modelo model)
    {
        return await client.PostAsJsonAsync("https://veiculos.fipe.org.br/api/veiculos/ConsultarAnoModelo",
        new
        {
            codigoTabelaReferencia = 323,
            codigoTipoVeiculo = 1,
            codigoMarca = brandId,
            codigoModelo = model.Value
        });
    }

    private static async Task<HttpResponseMessage> ConsultaCompleta(HttpClient client, int brandId, Modelo model, string[] anoCombustivel)
    {
        return await client.PostAsJsonAsync("https://veiculos.fipe.org.br/api/veiculos/ConsultarValorComTodosParametros",
        new
        {
            codigoTabelaReferencia = 323,
            codigoMarca = brandId,
            codigoModelo = model.Value,
            codigoTipoVeiculo = 1,
            anoModelo = int.Parse(anoCombustivel.First()),
            codigoTipoCombustivel = int.Parse(anoCombustivel.Last()),
            tipoConsulta = "tradicional"
        });
    }
}

public class CarInfos
{
    public int IdMarca { get; set; }
    public string NomeMarca { get; set; }
    public int IdModelo { get; set; }
    public string NomeModelo { get; set; }
    public int Ano { get; set; }
    public int TipoCombustivel { get; set; }
}

public readonly record struct VeiculoFipeResponse(
    string Valor,
    string Marca,
    string Modelo,
    int AnoModelo,
    string Combustivel,
    string CodigoFipe,
    string MesReferencia,
    string Autenticacao,
    int TipoVeiculo,
    string SiglaCombustivel,
    string DataConsulta
);


public class MarcaModelo
{
    public int IdMarca { get; set; }
    public string Nome { get; set; }
    public int Id { get; set; }
}

public class Modelo
{
    public string Label { get; set; }
    public int Value { get; set; }
}

public class Ano
{
    public string Label { get; set; }
    public string Value { get; set; }
}

public class ModelosResponse
{
    public List<Modelo> Modelos { get; set; }
}

public class AnosResponse
{
    public List<Ano> Anos { get; set; }
}