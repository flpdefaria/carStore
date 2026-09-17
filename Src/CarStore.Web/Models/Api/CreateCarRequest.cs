using System.ComponentModel.DataAnnotations;

namespace CarStore.Web.Models.Api;

public class CreateCarRequest
{
    [Required]
    public string Model { get; init; } = string.Empty;
    public string Vin { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string BodyType { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public int ModelYear { get; init; }
    [Required]
    public int BrandId { get; init; }
    public int Mileage { get; init; }
}
