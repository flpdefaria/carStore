using CarStore.Domain.Exceptions;

namespace CarStore.Domain.Entities;

public class Car
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Vin { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int ModelYear { get; set; }
    public bool IsAvailable => Stock > 0;
    public int Mileage { get; private set; }

    public int BrandId { get; set; }
    public Brand? Brand { get; set; }

    public static Car Create(string model, string vin, string description, string bodyType, decimal price, int stock, int modelYear, int brandId, int mileage)
    {
        Validate(model, price, stock, modelYear, mileage);
        return new Car
        {
            Model = model.Trim(),
            Vin = (vin ?? string.Empty).Trim(),
            Description = (description ?? string.Empty).Trim(),
            BodyType = (bodyType ?? string.Empty).Trim(),
            Price = price,
            Stock = stock,
            ModelYear = modelYear,
            BrandId = brandId,
            Mileage = mileage
        };
    }

    public void Update(string model, string vin, string description, string bodyType, decimal price, int stock, int modelYear, int brandId, int mileage)
    {
        Validate(model, price, stock, modelYear, mileage);
        Model = model.Trim();
        Vin = (vin ?? string.Empty).Trim();
        Description = (description ?? string.Empty).Trim();
        BodyType = (bodyType ?? string.Empty).Trim();
        Price = price;
        Stock = stock;
        ModelYear = modelYear;
        BrandId = brandId;
        Mileage = mileage;
    }

    private static void Validate(string model, decimal price, int stock, int modelYear, int mileage)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Model is required.");

        if (price < 0)
            throw new DomainException("Price cannot be negative.");

        if (stock < 0)
            throw new DomainException("Stock cannot be negative.");

        if (modelYear > DateTime.UtcNow.Year + 1)
            throw new DomainException("Model year cannot be in the future.");

        if (mileage < 0)
            throw new DomainException("Mileage cannot be negative.");
    }
}

