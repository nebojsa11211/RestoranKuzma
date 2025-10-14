using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        Console.WriteLine("Updating user passwords...");

        try
        {
            string connectionString = "Data Source=../src/RestaurantSuite.Api/restorankuzma.db";

            // Password to use: "Password123!"
            string password = "Password123!";
            string passwordHash = HashPassword(password);

            Console.WriteLine($"Generated password hash for '{password}': {passwordHash}");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // Update all test users with the same password
                using (var cmd = new SqliteCommand(@"
                    UPDATE Users
                    SET PasswordHash = @passwordHash
                    WHERE Email IN ('mark@restaurant.com', 'sarah@restaurant.com', 'david@restaurant.com', 'emma@restaurant.com')", connection))
                {
                    cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"\n✅ Updated {rowsAffected} user passwords");
                }

                // Display user information
                Console.WriteLine("\nTest Users (all with password: Password123!):");
                Console.WriteLine("=".PadRight(60, '='));
                using (var cmd = new SqliteCommand(@"
                    SELECT Email, FirstName, LastName, Role, IsActive
                    FROM Users
                    WHERE Email IN ('mark@restaurant.com', 'sarah@restaurant.com', 'david@restaurant.com', 'emma@restaurant.com')
                    ORDER BY Role", connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string email = reader.GetString(0);
                        string firstName = reader.GetString(1);
                        string lastName = reader.GetString(2);
                        int roleId = reader.GetInt32(3);
                        int isActive = reader.GetInt32(4);

                        string roleName = roleId switch
                        {
                            0 => "Admin",
                            1 => "Waiter",
                            2 => "Chef",
                            3 => "Guest",
                            _ => "Unknown"
                        };

                        Console.WriteLine($"  📧 {email}");
                        Console.WriteLine($"     Name: {firstName} {lastName}");
                        Console.WriteLine($"     Role: {roleName}");
                        Console.WriteLine($"     Active: {(isActive == 1 ? "Yes" : "No")}");
                        Console.WriteLine();
                    }
                }

                Console.WriteLine("✅ Password update completed successfully!");
                Console.WriteLine("\n🔑 Login Credentials for Chef Page:");
                Console.WriteLine("   Email: emma@restaurant.com");
                Console.WriteLine("   Password: Password123!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
