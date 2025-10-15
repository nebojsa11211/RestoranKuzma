#r "nuget: Microsoft.EntityFrameworkCore.Sqlite, 9.0.0"
#r "nuget: BCrypt.Net-Next, 4.0.3"
#load "../src/RestaurantSuite.Domain/Entities/User.cs"
#load "../src/RestaurantSuite.Domain/Enums/UserRole.cs"

using Microsoft.Data.Sqlite;
using BCrypt.Net;
using System;

var dbPath = "../src/RestaurantSuite.Api/restorankuzma.db";
var connectionString = $"Data Source={dbPath}";

Console.WriteLine($"Connecting to database: {dbPath}");

using var connection = new SqliteConnection(connectionString);
connection.Open();

// Check if admin user already exists
using (var checkCmd = connection.CreateCommand())
{
    checkCmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Email = @email";
    checkCmd.Parameters.AddWithValue("@email", "admin@restorankuzma.rs");
    var count = Convert.ToInt32(checkCmd.ExecuteScalar());

    if (count > 0)
    {
        Console.WriteLine("❌ Admin user already exists!");
        return;
    }
}

// Create admin user
var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
var now = DateTime.UtcNow;

using (var insertCmd = connection.CreateCommand())
{
    insertCmd.CommandText = @"
        INSERT INTO Users (Id, Email, FirstName, LastName, Phone, PasswordHash, Role, IsActive, CreatedAt, UpdatedAt)
        VALUES (@id, @email, @firstName, @lastName, @phone, @passwordHash, @role, @isActive, @createdAt, @updatedAt)";

    insertCmd.Parameters.AddWithValue("@id", Guid.NewGuid().ToString());
    insertCmd.Parameters.AddWithValue("@email", "admin@restorankuzma.rs");
    insertCmd.Parameters.AddWithValue("@firstName", "Admin");
    insertCmd.Parameters.AddWithValue("@lastName", "User");
    insertCmd.Parameters.AddWithValue("@phone", "+381111234567");
    insertCmd.Parameters.AddWithValue("@passwordHash", passwordHash);
    insertCmd.Parameters.AddWithValue("@role", 0); // Admin role = 0
    insertCmd.Parameters.AddWithValue("@isActive", 1);
    insertCmd.Parameters.AddWithValue("@createdAt", now.ToString("yyyy-MM-dd HH:mm:ss"));
    insertCmd.Parameters.AddWithValue("@updatedAt", now.ToString("yyyy-MM-dd HH:mm:ss"));

    insertCmd.ExecuteNonQuery();
}

Console.WriteLine("✅ Admin user created successfully!");
Console.WriteLine("   Email: admin@restorankuzma.rs");
Console.WriteLine("   Password: Admin123!");
