using RestaurantSuite.Domain.Enums;

namespace RestaurantSuite.Domain.Entities;

public class Table
{
    public Guid Id { get; private set; }
    public string TableNumber { get; private set; }
    public int Capacity { get; private set; }
    public TableStatus Status { get; private set; }
    public Guid QRCodeIdentifier { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Table() { }

    public static Table Create(string tableNumber, int capacity)
    {
        if (string.IsNullOrWhiteSpace(tableNumber))
            throw new ArgumentException("Table number cannot be empty", nameof(tableNumber));

        if (capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero", nameof(capacity));

        return new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = tableNumber,
            Capacity = capacity,
            Status = TableStatus.Available,
            QRCodeIdentifier = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsOccupied()
    {
        Status = TableStatus.Occupied;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsReserved()
    {
        Status = TableStatus.Reserved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsAvailable()
    {
        Status = TableStatus.Available;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkForCleaning()
    {
        Status = TableStatus.Cleaning;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCapacity(int newCapacity)
    {
        if (newCapacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero", nameof(newCapacity));

        Capacity = newCapacity;
        UpdatedAt = DateTime.UtcNow;
    }
}
