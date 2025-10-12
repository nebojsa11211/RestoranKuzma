using MediatR;

namespace RestaurantSuite.Application.Commands;

public class CreateRestaurantCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
}
