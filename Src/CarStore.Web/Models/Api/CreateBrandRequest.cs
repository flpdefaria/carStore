using System.ComponentModel.DataAnnotations;

namespace CarStore.Web.Models.Api;

public class CreateBrandRequest
{
    [Required]
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public DateTime FoundedDate { get; init; }
}
