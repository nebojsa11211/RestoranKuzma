using FluentAssertions;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Domain;

public class TableTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateTable()
    {
        // Arrange
        var tableNumber = "T1";
        var capacity = 4;

        // Act
        var table = RestaurantSuite.Domain.Entities.Table.Create(tableNumber, capacity);

        // Assert
        table.Should().NotBeNull();
        table.TableNumber.Should().Be(tableNumber);
        table.Capacity.Should().Be(capacity);
        table.Status.Should().Be(RestaurantSuite.Domain.Enums.TableStatus.Available);
        table.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidCapacity_ShouldThrowException(int invalidCapacity)
    {
        // Act
        var act = () => RestaurantSuite.Domain.Entities.Table.Create("T1", invalidCapacity);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithInvalidTableNumber_ShouldThrowException(string invalidTableNumber)
    {
        // Act
        var act = () => RestaurantSuite.Domain.Entities.Table.Create(invalidTableNumber, 4);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MarkAsOccupied_WhenAvailable_ShouldChangeStatusToOccupied()
    {
        // Arrange
        var table = RestaurantSuite.Domain.Entities.Table.Create("T1", 4);

        // Act
        table.MarkAsOccupied();

        // Assert
        table.Status.Should().Be(RestaurantSuite.Domain.Enums.TableStatus.Occupied);
    }

    [Fact]
    public void MarkAsReserved_WhenAvailable_ShouldChangeStatusToReserved()
    {
        // Arrange
        var table = RestaurantSuite.Domain.Entities.Table.Create("T1", 4);

        // Act
        table.MarkAsReserved();

        // Assert
        table.Status.Should().Be(RestaurantSuite.Domain.Enums.TableStatus.Reserved);
    }

    [Fact]
    public void MarkAsAvailable_WhenOccupied_ShouldChangeStatusToAvailable()
    {
        // Arrange
        var table = RestaurantSuite.Domain.Entities.Table.Create("T1", 4);
        table.MarkAsOccupied();

        // Act
        table.MarkAsAvailable();

        // Assert
        table.Status.Should().Be(RestaurantSuite.Domain.Enums.TableStatus.Available);
    }
}
