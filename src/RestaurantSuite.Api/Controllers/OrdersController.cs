using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Queries;

namespace RestaurantSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetOrdersByRestaurantQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetOrderByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new { message = $"Order with ID {id} not found" });

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var command = new UpdateOrderStatusCommand
        {
            OrderId = id,
            Action = request.Action
        };

        await _mediator.Send(command);
        return NoContent();
    }
}

public class UpdateOrderStatusRequest
{
    public string Action { get; set; } = string.Empty;
}
