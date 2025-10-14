using Microsoft.Data.Sqlite;

var connectionString = @"Data Source=src/RestaurantSuite.Api/restorankuzma.db";

using var connection = new SqliteConnection(connectionString);
connection.Open();

var command = connection.CreateCommand();
command.CommandText = @"
    SELECT Role, COUNT(*) as Count
    FROM Users
    GROUP BY Role
    ORDER BY Role";

Console.WriteLine("Users by Role:");
Console.WriteLine("==================");

using (var reader = command.ExecuteReader())
{
    while (reader.Read())
    {
        var role = reader.GetInt32(0);
        var count = reader.GetInt32(1);
        var roleName = role switch
        {
            0 => "Admin",
            1 => "Waiter",
            2 => "Chef",
            3 => "Guest",
            _ => "Unknown"
        };
        Console.WriteLine($"{roleName} (Role {role}): {count} users");
    }
}

Console.WriteLine("==================");

// Get total count
command.CommandText = "SELECT COUNT(*) FROM Users";
var totalCount = (long)command.ExecuteScalar()!;
Console.WriteLine($"Total users: {totalCount}");

// Verify expected counts
var expected = new Dictionary<string, int>
{
    {"Admin", 2},
    {"Waiter", 16},
    {"Chef", 4},
    {"Guest", 300}
};

var expectedTotal = expected.Values.Sum();
Console.WriteLine("\n✅ VERIFICATION:");
Console.WriteLine($"Expected total: {expectedTotal}");
Console.WriteLine($"Actual total: {totalCount}");
Console.WriteLine(totalCount == expectedTotal ? "✅ SUCCESS: All users inserted correctly!" : "❌ FAILED: User count mismatch");
