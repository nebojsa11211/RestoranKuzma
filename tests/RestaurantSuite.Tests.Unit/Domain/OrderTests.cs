using FluentAssertions;
using Xunit;

namespace RestaurantSuite.Tests.Unit.Domain;

public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        var tableId = Guid.NewGuid();
        var waiterId = Guid.NewGuid();

        // Act
        var order = RestaurantSuite.Domain.Entities.Order.Create(tableId, waiterId);

        // Assert
        order.Should().NotBeNull();
        order.TableId.Should().Be(tableId);
        order.WaiterId.Should().Be(waiterId);
        order.Status.Should().Be(RestaurantSuite.Domain.Enums.OrderStatus.Pending);
        order.OrderItems.Should().BeEmpty();
        order.TotalAmount.Should().Be(0);
        order.Id.Should().NotBeEmpty();
        order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void AddItem_WithValidItem_ShouldAddToOrderItems()
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var menuItemId = Guid.NewGuid();
        var quantity = 2;
        var unitPrice = 10.00m;

        // Act
        order.AddItem(menuItemId, quantity, unitPrice, "No onions");

        // Assert
        order.OrderItems.Should().HaveCount(1);
        var orderItem = order.OrderItems.First();
        orderItem.MenuItemId.Should().Be(menuItemId);
        orderItem.Quantity.Should().Be(quantity);
        orderItem.UnitPrice.Should().Be(unitPrice);
        orderItem.SpecialInstructions.Should().Be("No onions");
        order.TotalAmount.Should().Be(20.00m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_WithInvalidQuantity_ShouldThrowException(int invalidQuantity)
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        var act = () => order.AddItem(Guid.NewGuid(), invalidQuantity, 10.00m, null);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ConfirmOrder_WhenPending_ShouldChangeStatusToConfirmed()
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        order.ConfirmOrder();

        // Assert
        order.Status.Should().Be(RestaurantSuite.Domain.Enums.OrderStatus.Confirmed);
    }

    [Fact]
    public void StartPreparation_WhenConfirmed_ShouldChangeStatusToInProgress()
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        order.ConfirmOrder();

        // Act
        order.StartPreparation();

        // Assert
        order.Status.Should().Be(RestaurantSuite.Domain.Enums.OrderStatus.InProgress);
    }

    [Fact]
    public void CompletePreparation_WhenInProgress_ShouldChangeStatusToReady()
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        order.ConfirmOrder();
        order.StartPreparation();

        // Act
        order.CompletePreparation();

        // Assert
        order.Status.Should().Be(RestaurantSuite.Domain.Enums.OrderStatus.Ready);
    }

    [Fact]
    public void ServeOrder_WhenReady_ShouldChangeStatusToServed()
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        order.ConfirmOrder();
        order.StartPreparation();
        order.CompletePreparation();

        // Act
        order.ServeOrder();

        // Assert
        order.Status.Should().Be(RestaurantSuite.Domain.Enums.OrderStatus.Served);
    }

    [Fact]
    public void CompleteOrder_WhenServed_ShouldChangeStatusToCompleted()
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        order.ConfirmOrder();
        order.StartPreparation();
        order.CompletePreparation();
        order.ServeOrder();

        // Act
        order.CompleteOrder();

        // Assert
        order.Status.Should().Be(RestaurantSuite.Domain.Enums.OrderStatus.Completed);
        order.CompletedAt.Should().NotBeNull();
        order.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CancelOrder_WhenNotCompleted_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        order.CancelOrder();

        // Assert
        order.Status.Should().Be(RestaurantSuite.Domain.Enums.OrderStatus.Cancelled);
    }

    [Fact]
    public void RemoveItem_WithExistingOrderItemId_ShouldRemoveItem()
    {
        // Arrange
        var order = RestaurantSuite.Domain.Entities.Order.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        order.AddItem(Guid.NewGuid(), 2, 10.00m, null);
        var orderItemId = order.OrderItems.First().Id;

        // Act
        order.RemoveItem(orderItemId);

        // Assert
        order.OrderItems.Should().BeEmpty();
        order.TotalAmount.Should().Be(0);
    }
}
