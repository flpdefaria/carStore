using CarStore.Domain.Entities;
using CarStore.Domain.Exceptions;
using FluentAssertions;

namespace CarStore.Tests;

public class CarTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static Car ValidCar(int mileage = 200) =>
        Car.Create("Model 3", "5YJ3E1EA9NF123456", "All-electric mass-market sedan.", "Sedan",
            41000m, 10, DateTime.UtcNow.Year, 1, mileage);

    // ── Car.Create – Mileage happy path ──────────────────────────────────────

    [Fact]
    public void Create_WithValidMileage_Succeeds()
    {
        // Arrange & Act
        var car = Car.Create("Model 3", "5YJ3E1EA9NF123456", "All-electric mass-market sedan.", "Sedan",
            41000m, 10, DateTime.UtcNow.Year, 1, 300);

        // Assert
        car.Should().NotBeNull();
        car.Mileage.Should().Be(300);
    }

    // ── Car.Create – Mileage validation ─────────────────────────────────────

    [Fact]
    public void Create_WithMileageZero_Succeeds()
    {
        // Arrange & Act
        var car = Car.Create("Model 3", "5YJ3E1EA9NF123456", "All-electric mass-market sedan.", "Sedan",
            41000m, 10, DateTime.UtcNow.Year, 1, 0);

        // Assert
        car.Mileage.Should().Be(0);
    }

    [Fact]
    public void Create_WithNegativeMileage_ThrowsDomainException()
    {
        // Arrange
        Action act = () => Car.Create("Model 3", "5YJ3E1EA9NF123456", "All-electric mass-market sedan.", "Sedan",
            41000m, 10, DateTime.UtcNow.Year, 1, -1);

        // Act & Assert
        act.Should().Throw<DomainException>().WithMessage("Mileage cannot be negative.");
    }

    [Fact]
    public void Create_WithModelYearTooFarInFuture_ThrowsDomainException()
    {
        // Arrange
        Action act = () => Car.Create("Model 3", "5YJ3E1EA9NF123456", "All-electric mass-market sedan.", "Sedan",
            41000m, 10, DateTime.UtcNow.Year + 2, 1, 0);

        // Act & Assert
        act.Should().Throw<DomainException>().WithMessage("Model year cannot be in the future.");
    }

    // ── Car.Update – Mileage happy path ─────────────────────────────────────

    [Fact]
    public void Update_WithValidMileage_Succeeds()
    {
        // Arrange
        var car = ValidCar();

        // Act
        car.Update("Model 3", "5YJ3E1EA9NF123456", "All-electric mass-market sedan.", "Sedan",
            41000m, 10, DateTime.UtcNow.Year, 1, 500);

        // Assert
        car.Mileage.Should().Be(500);
    }
}

