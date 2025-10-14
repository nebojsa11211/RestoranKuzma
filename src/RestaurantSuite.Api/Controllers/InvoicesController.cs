using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Application.Queries;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IInvoiceService _invoiceService;
    private readonly ILogger<InvoicesController> _logger;

    public InvoicesController(
        IMediator mediator,
        IInvoiceService invoiceService,
        ILogger<InvoicesController> logger)
    {
        _mediator = mediator;
        _invoiceService = invoiceService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll()
    {
        try
        {
            var query = new GetAllInvoicesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoices");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceDto>> GetById(Guid id)
    {
        try
        {
            var query = new GetInvoiceByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { message = $"Invoice with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice with id {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<InvoiceDto>> GetByOrderId(Guid orderId)
    {
        try
        {
            var query = new GetInvoiceByOrderIdQuery { OrderId = orderId };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { message = $"Invoice for order {orderId} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice for order {OrderId}", orderId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateFromOrder([FromBody] CreateInvoiceFromOrderCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid invoice creation request");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("{id}/issue")]
    public async Task<IActionResult> Issue(Guid id)
    {
        try
        {
            var command = new IssueInvoiceCommand { InvoiceId = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid invoice issue request for {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error issuing invoice {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordPaymentRequest request)
    {
        try
        {
            var command = new RecordInvoicePaymentCommand
            {
                InvoiceId = id,
                PaymentId = request.PaymentId
            };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid payment recording for invoice {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording payment for invoice {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var command = new CancelInvoiceCommand { InvoiceId = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid cancel request for invoice {Id}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling invoice {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}/html")]
    public async Task<IActionResult> GetInvoiceHtml(Guid id)
    {
        try
        {
            var html = await _invoiceService.GenerateInvoiceHtmlAsync(id);
            return Content(html, "text/html");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invoice {Id} not found", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating HTML for invoice {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetInvoicePdf(Guid id)
    {
        try
        {
            var pdf = await _invoiceService.GenerateInvoicePdfAsync(id);
            return File(pdf, "application/pdf", $"invoice-{id}.pdf");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invoice {Id} not found", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF for invoice {Id}", id);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

public class RecordPaymentRequest
{
    public Guid PaymentId { get; set; }
}
