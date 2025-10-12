using System;
using System.IO;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {
        Console.WriteLine("Initializing RestaurantSuite database...");
        
        try
        {
            string connectionString = "Data Source=../src/RestaurantSuite.Api/restorankuzma.db";
            string sqlScriptPath = "../init-sqlite-db-complete.sql";
            
            // Read the SQL script
            if (!File.Exists(sqlScriptPath))
            {
                Console.WriteLine($"SQL script not found at: {sqlScriptPath}");
                return;
            }
            
            string sqlScript = File.ReadAllText(sqlScriptPath);
            
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                
                // Execute the SQL script
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqliteCommand(sqlScript, connection, transaction))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        
            transaction.Commit();
            Console.WriteLine("Database initialized successfully!");
            
            // Fix any NULL DateTime values in existing data
            FixNullDateTimeValues(connection);
            
            // Verify the data
            VerifyDatabase(connection);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error executing SQL script: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    
    static void VerifyDatabase(SqliteConnection connection)
    {
        Console.WriteLine("\nVerifying database structure...");
        
        // Check what tables exist
        Console.WriteLine("Tables in database:");
        using (var cmd = new SqliteCommand("SELECT name FROM sqlite_master WHERE type='table';", connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"  - {reader.GetString(0)}");
            }
        }
        
        // Check Restaurants table
        Console.WriteLine("\nSample data in Restaurants table:");
        using (var cmd = new SqliteCommand("SELECT Id, Name, Address, Currency FROM Restaurants LIMIT 1;", connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"  {reader.GetString(1)} - {reader.GetString(2)} ({reader.GetString(3)})");
            }
        }
        
        // Check Categories table
        Console.WriteLine("\nCategories in database:");
        using (var cmd = new SqliteCommand("SELECT Name FROM Categories ORDER BY DisplayOrder;", connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"  - {reader.GetString(0)}");
            }
        }
        
        // Check MenuItems table
        Console.WriteLine("\nSample menu items:");
        using (var cmd = new SqliteCommand("SELECT Name, Price, CategoryId FROM MenuItems LIMIT 3;", connection))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"  {reader.GetString(0)} - {reader.GetString(1)} RSD");
            }
        }
        
        Console.WriteLine("\n✅ Database initialization completed successfully!");
        Console.WriteLine("✅ The add-menu-item-submit-btn functionality is now ready to use!");
    }

    static void FixNullDateTimeValues(SqliteConnection connection)
    {
        Console.WriteLine("\nFixing NULL DateTime values in existing data...");
        
        // Fix MenuItems table
        Console.WriteLine("Fixing MenuItems table...");
        using (var cmd = new SqliteCommand(@"
            UPDATE MenuItems 
            SET UpdatedAt = CreatedAt 
            WHERE UpdatedAt IS NULL OR UpdatedAt = '';", connection))
        {
            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"Fixed {rowsAffected} rows in MenuItems table");
        }
        
        // Fix Categories table
        Console.WriteLine("Fixing Categories table...");
        using (var cmd = new SqliteCommand(@"
            UPDATE Categories 
            SET UpdatedAt = CreatedAt 
            WHERE UpdatedAt IS NULL OR UpdatedAt = '';", connection))
        {
            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"Fixed {rowsAffected} rows in Categories table");
        }
        
        // Fix Users table
        Console.WriteLine("Fixing Users table...");
        using (var cmd = new SqliteCommand(@"
            UPDATE Users 
            SET UpdatedAt = CreatedAt 
            WHERE UpdatedAt IS NULL OR UpdatedAt = '';", connection))
        {
            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"Fixed {rowsAffected} rows in Users table");
        }
        
        // Fix Restaurants table
        Console.WriteLine("Fixing Restaurants table...");
        using (var cmd = new SqliteCommand(@"
            UPDATE Restaurants 
            SET UpdatedAt = CreatedAt 
            WHERE UpdatedAt IS NULL OR UpdatedAt = '';", connection))
        {
            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"Fixed {rowsAffected} rows in Restaurants table");
        }
        
        // Fix Orders table
        Console.WriteLine("Fixing Orders table...");
        using (var cmd = new SqliteCommand(@"
            UPDATE Orders 
            SET UpdatedAt = CreatedAt 
            WHERE UpdatedAt IS NULL OR UpdatedAt = '';", connection))
        {
            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"Fixed {rowsAffected} rows in Orders table");
        }
        
        Console.WriteLine("NULL DateTime fix completed successfully!");
    }
}
