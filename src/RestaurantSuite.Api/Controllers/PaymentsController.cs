using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.Commands;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentsController> _logger;
    private readonly IMediator _mediator;

    public PaymentsController(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        ILogger<PaymentsController> logger,
        IMediator mediator)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Payment>>> GetAll()
    {
        try
        {
            var payments = await _paymentRepository.GetAllAsync();
            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Payment>> GetById(Guid id)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment with id {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<IEnumerable<Payment>>> GetByOrderId(Guid orderId)
    {
        try
        {
            var payments = await _paymentRepository.GetByOrderIdAsync(orderId);
            return Ok(payments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for order {OrderId}", orderId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Payment>> Create([FromBody] CreatePaymentRequest request)
    {
        try
        {
            // Verify order exists
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                return NotFound($"Order with id {request.OrderId} not found");
            }

            var payment = Payment.Create(
                request.OrderId,
                request.Method,
                request.Amount,
                request.TipAmount,
                request.Notes
            );

            await _paymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            // Automatically create invoice for this payment
            try
            {
                var createInvoiceCommand = new CreateInvoiceFromOrderCommand
                {
                    OrderId = request.OrderId,
                    TaxRate = 0.08m, // 8% default tax rate
                    TipAmount = request.TipAmount,
                    AutoIssue = true
                };

                var invoiceId = await _mediator.Send(createInvoiceCommand);
                _logger.LogInformation("Created invoice {InvoiceId} for payment {PaymentId}", invoiceId, payment.Id);

                // Link payment to invoice
                var recordPaymentCommand = new RecordInvoicePaymentCommand
                {
                    InvoiceId = invoiceId,
                    PaymentId = payment.Id
                };
                await _mediator.Send(recordPaymentCommand);
            }
            catch (InvalidOperationException ex)
            {
                // Invoice might already exist, log but don't fail the payment
                _logger.LogWarning(ex, "Could not create invoice for payment {PaymentId}", payment.Id);
            }

            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid payment request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("split")]
    public async Task<ActionResult<IEnumerable<Payment>>> CreateSplitPayment([FromBody] SplitPaymentRequest request)
    {
        try
        {
            // Verify order exists
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                return NotFound($"Order with id {request.OrderId} not found");
            }

            var payments = new List<Payment>();
            foreach (var item in request.SplitItems)
            {
                var payment = Payment.Create(
                    request.OrderId,
                    item.Method,
                    item.Amount,
                    item.TipAmount,
                    item.Notes
                );

                await _paymentRepository.AddAsync(payment);
                payments.Add(payment);
            }

            await _unitOfWork.SaveChangesAsync();

            return Ok(payments);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid split payment request");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating split payment");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(Guid id, [FromBody] CompletePaymentRequest? request = null)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            payment.MarkAsCompleted(request?.TransactionId);
            _paymentRepository.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid payment status transition for payment {Id}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing payment {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}/fail")]
    public async Task<IActionResult> Fail(Guid id)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            payment.MarkAsFailed();
            _paymentRepository.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid payment status transition for payment {Id}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking payment {Id} as failed", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}/refund")]
    public async Task<IActionResult> Refund(Guid id)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            payment.Refund();
            _paymentRepository.Update(payment);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid payment status transition for payment {Id}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}

public class CreatePaymentRequest
{
    public Guid OrderId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public decimal TipAmount { get; set; }
    public string? Notes { get; set; }
}

public class SplitPaymentRequest
{
    public Guid OrderId { get; set; }
    public List<SplitPaymentItem> SplitItems { get; set; } = new();
}

public class SplitPaymentItem
{
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public decimal TipAmount { get; set; }
    public string? Notes { get; set; }
}

public class CompletePaymentRequest
{
    public string? TransactionId { get; set; }
}
