namespace CarStore.Web.Models.Api;

public class CarDto
{
    public int Id { get; init; }
    public string Model { get; init; } = string.Empty;
    public string Vin { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string BodyType { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public int Mileage { get; init; }
    public bool IsAvailable { get; init; }
    public int ModelYear { get; init; }
    public int BrandId { get; init; }
    public string BrandName { get; init; } = string.Empty;
}
