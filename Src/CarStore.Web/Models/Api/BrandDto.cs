namespace CarStore.Web.Models.Api;

public class BrandDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public DateTime FoundedDate { get; init; }
    public int YearsInBusiness { get; init; }
    public int CarsCount { get; init; }
    public List<BrandCarSummaryDto> Cars { get; init; } = new();
}

public class BrandCarSummaryDto
{
    public string Model { get; init; } = string.Empty;
    public int ModelYear { get; init; }
}
