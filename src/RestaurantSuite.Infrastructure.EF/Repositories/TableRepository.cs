using Microsoft.EntityFrameworkCore;
using RestaurantSuite.Application.Interfaces;
using RestaurantSuite.Domain.Entities;
using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Infrastructure.EF.Repositories;

public class TableRepository : ITableRepository
{
    private readonly ApplicationDbContext _context;

    public TableRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Table?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tables.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<Table>> GetByRestaurantIdAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        // Single-restaurant architecture: no RestaurantId filtering needed
        return await _context.Tables
            .OrderBy(t => t.TableNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Table>> GetByStatusAsync(Guid restaurantId, TableStatus status, CancellationToken cancellationToken = default)
    {
        // Single-restaurant architecture: no RestaurantId filtering needed
        return await _context.Tables
            .Where(t => t.Status == status)
            .OrderBy(t => t.TableNumber)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Table table, CancellationToken cancellationToken = default)
    {
        await _context.Tables.AddAsync(table, cancellationToken);
    }

    public void Update(Table table)
    {
        _context.Tables.Update(table);
    }

    public void Delete(Table table)
    {
        _context.Tables.Remove(table);
    }
}
