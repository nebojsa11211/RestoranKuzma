using Microsoft.AspNetCore.Mvc;
using RestaurantSuite.Application.DTOs;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Application.Queries;
using RestaurantSuite.Domain.Entities;

namespace RestaurantSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly ITableRepository _tableRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TablesController> _logger;

    public TablesController(ITableRepository tableRepository, IUnitOfWork unitOfWork, ILogger<TablesController> logger)
    {
        _tableRepository = tableRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<TableDto>>> GetTables()
    {
        try
        {
            // Get all tables - single restaurant architecture, no RestaurantId filtering needed
            var tables = await _tableRepository.GetByRestaurantIdAsync(Guid.Empty);
            var tableDtos = tables.Select(t => new TableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                Status = t.Status
            }).ToList();

            return Ok(tableDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tables");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TableDto>> GetTable(Guid id)
    {
        try
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            var tableDto = new TableDto
            {
                Id = table.Id,
                TableNumber = table.TableNumber,
                Capacity = table.Capacity,
                Status = table.Status
            };

            return Ok(tableDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving table with id {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<CreateTableResponse>> CreateTable([FromBody] CreateTableCommand command)
    {
        try
        {
            var table = Table.Create(command.TableNumber, command.Capacity);
            
            await _tableRepository.AddAsync(table);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTable), new { id = table.Id }, new CreateTableResponse { Id = table.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating table");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}/reserve")]
    public async Task<IActionResult> ReserveTable(Guid id)
    {
        try
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            if (table.Status != Domain.Enums.TableStatus.Available)
            {
                return BadRequest("Table is not available for reservation");
            }

            table.MarkAsReserved();
            _tableRepository.Update(table);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving table with id {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}/occupy")]
    public async Task<IActionResult> OccupyTable(Guid id)
    {
        try
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            table.MarkAsOccupied();
            _tableRepository.Update(table);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occupying table with id {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}/available")]
    public async Task<IActionResult> MakeTableAvailable(Guid id)
    {
        try
        {
            var table = await _tableRepository.GetByIdAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            table.MarkAsAvailable();
            _tableRepository.Update(table);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error making table available with id {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
