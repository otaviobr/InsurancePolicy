namespace InsurancePolicy.Domain.Entities;

public class CarEntity
{
    public string Id { get; set; }
    public decimal Price { get; set; }
    public int ModelId { get; set; }
    public string ModelName { get; set; }
    public int BrandId { get; set; }
    public string BrandName { get; set; }
    public int Year { get; set; }
    public int FuelType { get; set; }
}