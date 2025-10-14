namespace RestaurantSuite.Guest.Models
{
    public class GuestMenuItemDto
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public int? PreparationTimeMinutes { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<string> MainIngredients { get; set; } = new(); // Just names, no quantities
    }
}
