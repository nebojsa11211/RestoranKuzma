using System;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        Console.WriteLine("Fixing user roles in database...");
        Console.WriteLine("\nUserRole Enum:");
        Console.WriteLine("  Admin = 0");
        Console.WriteLine("  Waiter = 1");
        Console.WriteLine("  Chef = 2");
        Console.WriteLine("  Guest = 3");

        string connectionString = "Data Source=../src/RestaurantSuite.Api/restorankuzma.db";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            Console.WriteLine("\n\nBefore fix:");
            ShowUsers(connection);

            // Update roles to match the enum correctly
            Console.WriteLine("\n\nUpdating roles...");

            // mark@restaurant.com should be Waiter (1), not Admin (0)
            UpdateUserRole(connection, "mark@restaurant.com", 1, "Waiter");

            // sarah@restaurant.com should be Chef (2), not Waiter (1)
            UpdateUserRole(connection, "sarah@restaurant.com", 2, "Chef");

            // david@restaurant.com should be Waiter (1), not Admin (0)
            UpdateUserRole(connection, "david@restaurant.com", 1, "Waiter");

            // emma@restaurant.com should be Admin (0), not Chef (2)
            UpdateUserRole(connection, "emma@restaurant.com", 0, "Admin");

            Console.WriteLine("\n\nAfter fix:");
            ShowUsers(connection);
        }

        Console.WriteLine("\n✓ User roles have been fixed!");
        Console.WriteLine("\nYou can now login with:");
        Console.WriteLine("  • mark@restaurant.com (password123) - Waiter");
        Console.WriteLine("  • david@restaurant.com (password123) - Waiter");
        Console.WriteLine("  • sarah@restaurant.com (password123) - Chef");
        Console.WriteLine("  • emma@restaurant.com (password123) - Admin");
    }

    static void UpdateUserRole(SqliteConnection connection, string email, int role, string roleName)
    {
        using (var cmd = new SqliteCommand(@"
            UPDATE Users
            SET Role = @role
            WHERE Email = @email", connection))
        {
            cmd.Parameters.AddWithValue("@role", role);
            cmd.Parameters.AddWithValue("@email", email);
            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"  Updated {email} to {roleName} (Role={role})");
        }
    }

    static void ShowUsers(SqliteConnection connection)
    {
        using (var cmd = new SqliteCommand("SELECT Email, Role, FirstName, LastName FROM Users ORDER BY Email;", connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                string email = reader.GetString(0);
                int role = reader.GetInt32(1);
                string firstName = reader.GetString(2);
                string lastName = reader.GetString(3);
                string roleName = role switch
                {
                    0 => "Admin",
                    1 => "Waiter",
                    2 => "Chef",
                    3 => "Guest",
                    _ => "Unknown"
                };
                Console.WriteLine($"  {email,-30} Role={role} ({roleName,-10}) {firstName} {lastName}");
            }
        }
    }
}
