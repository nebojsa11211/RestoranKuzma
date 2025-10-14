using System.Text.Json.Serialization;

namespace RestaurantSuite.Guest.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CustomizationType
    {
        More = 0,
        Less = 1
    }

    public class OrderItemCustomization
    {
        public string IngredientName { get; set; } = string.Empty;
        public CustomizationType CustomizationType { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateOrderItemRequest
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? SpecialInstructions { get; set; }
        public List<OrderItemCustomization> Customizations { get; set; } = new();
    }

    public class CreateOrderRequest
    {
        public Guid TableId { get; set; }
        public Guid WaiterId { get; set; }
        public Guid? GuestId { get; set; }
        public List<CreateOrderItemRequest> Items { get; set; } = new();
    }
}
