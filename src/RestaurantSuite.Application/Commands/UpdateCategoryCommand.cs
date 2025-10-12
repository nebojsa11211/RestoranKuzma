using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to update a category
/// </summary>
public class UpdateCategoryCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
