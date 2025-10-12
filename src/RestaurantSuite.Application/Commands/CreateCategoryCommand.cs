using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to create a new category
/// </summary>
public class CreateCategoryCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
