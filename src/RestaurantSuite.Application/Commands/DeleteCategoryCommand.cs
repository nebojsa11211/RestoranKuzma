using MediatR;

namespace RestaurantSuite.Application.Commands;

/// <summary>
/// Command to delete a category
/// </summary>
public class DeleteCategoryCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
}
