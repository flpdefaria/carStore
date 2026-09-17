using CarStore.Domain.Entities;
using CarStore.Domain.Exceptions;
using FluentAssertions;

namespace CarStore.Tests;

public class BrandTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static Brand ValidBrand(DateTime? foundedDate = null) =>
        Brand.Create("Toyota", "Japanese multinational automaker.", "Japan",
            foundedDate ?? DateTime.UtcNow.AddYears(-50));

    // ── Brand.Create – happy path ───────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_ReturnsNonNullBrand()
    {
        // Arrange & Act
        var brand = Brand.Create("Ford", "American automaker.", "USA",
            DateTime.UtcNow.AddYears(-40));

        // Assert
        brand.Should().NotBeNull();
        brand.Name.Should().Be("Ford");
    }

    // ── Brand.Create – validation ───────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespaceName_ThrowsDomainException(string name)
    {
        // Arrange & Act
        var act = () => Brand.Create(name, "Description", "USA", DateTime.UtcNow.AddYears(-30));

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Brand name is required.");
    }

    [Fact]
    public void Create_WithFutureFoundedDate_ThrowsDomainException()
    {
        // Arrange & Act
        var act = () => Brand.Create("Valid Name", "Description", "USA",
            DateTime.UtcNow.AddDays(1));

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Founded date cannot be in the future.");
    }

    // ── Brand.YearsInBusiness – computed property ───────────────────────────

    [Fact]
    public void YearsInBusiness_WithFoundedDateExactly30YearsAgo_Returns30()
    {
        // Arrange
        // AddYears(-30).AddDays(-1) ensures we are one day past the 30-year mark,
        // making the formula (TotalDays / 365.25) reliably floor to 30 regardless
        // of how many leap years fall in the range.
        var foundedDate = DateTime.UtcNow.AddYears(-30).AddDays(-1);
        var brand = Brand.Create("Test Brand", "Description", "USA", foundedDate);

        // Act & Assert
        brand.YearsInBusiness.Should().Be(30);
    }

    // ── Brand.EnsureCanBeDeleted ─────────────────────────────────────────────

    [Fact]
    public void EnsureCanBeDeleted_WithCars_ThrowsDomainException()
    {
        // Arrange
        var brand = ValidBrand();
        brand.Cars.Add(new Car()); // add a car directly to the collection

        // Act
        var act = () => brand.EnsureCanBeDeleted();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot delete a brand that still has cars.");
    }
}
