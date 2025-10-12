using RestaurantSuite.Domain.Enums;
using System.Text.Json.Serialization;

namespace RestaurantSuite.Application.DTOs;

public class TableDto
{
    public Guid Id { get; set; }
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TableStatus Status { get; set; }
}

public class CreateTableCommand
{
    public string TableNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
}

public class CreateTableResponse
{
    public Guid Id { get; set; }
}
