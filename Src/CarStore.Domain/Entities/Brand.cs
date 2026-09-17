using CarStore.Domain.Exceptions;

namespace CarStore.Domain.Entities;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime FoundedDate { get; set; }
    public string Country { get; set; } = string.Empty;

    public ICollection<Car> Cars { get; set; } = new List<Car>();

    public int YearsInBusiness => (int)Math.Floor((DateTime.UtcNow - FoundedDate).TotalDays / 365.25);

    public static Brand Create(string name, string description, string country, DateTime foundedDate)
    {
        Validate(name, foundedDate);
        return new Brand
        {
            Name = name.Trim(),
            Description = (description ?? string.Empty).Trim(),
            Country = (country ?? string.Empty).Trim(),
            FoundedDate = foundedDate
        };
    }

    public void Update(string name, string description, string country, DateTime foundedDate)
    {
        Validate(name, foundedDate);
        Name = name.Trim();
        Description = (description ?? string.Empty).Trim();
        Country = (country ?? string.Empty).Trim();
        FoundedDate = foundedDate;
    }

    public void EnsureCanBeDeleted()
    {
        if (Cars.Any())
            throw new DomainException("Cannot delete a brand that still has cars.");
    }

    private static void Validate(string name, DateTime foundedDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Brand name is required.");

        if (name.Length > 150)
            throw new DomainException("Brand name must be at most 150 characters.");

        if (foundedDate > DateTime.UtcNow)
            throw new DomainException("Founded date cannot be in the future.");
    }
}
