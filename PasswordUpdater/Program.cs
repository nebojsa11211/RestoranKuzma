using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        Console.WriteLine("Updating user passwords...");

        string connectionString = "Data Source=../src/RestaurantSuite.Api/restorankuzma.db";
        string password = "password123"; // Default test password for all users
        string passwordHash = HashPassword(password);

        Console.WriteLine($"Password hash: {passwordHash}");

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var cmd = new SqliteCommand(@"
                UPDATE Users
                SET PasswordHash = @passwordHash", connection))
            {
                cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                int rowsAffected = cmd.ExecuteNonQuery();
                Console.WriteLine($"Updated {rowsAffected} user accounts");
            }

            // Verify the update
            Console.WriteLine("\nUser accounts:");
            using (var cmd = new SqliteCommand("SELECT Email, Role, IsActive FROM Users;", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"  - {reader.GetString(0)} (Role: {reader.GetInt32(1)}, Active: {reader.GetInt32(2) == 1})");
                }
            }
        }

        Console.WriteLine($"\n✓ All users now have the password: {password}");
        Console.WriteLine("\nAvailable accounts:");
        Console.WriteLine("  • mark@restaurant.com - Waiter (Role 0)");
        Console.WriteLine("  • sarah@restaurant.com - Chef (Role 1)");
        Console.WriteLine("  • david@restaurant.com - Waiter (Role 0)");
        Console.WriteLine("  • emma@restaurant.com - Admin (Role 2)");
        Console.WriteLine("\nYou can now login at http://localhost:5235 with any of these accounts.");
    }

    static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
