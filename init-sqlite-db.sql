-- SQLite Database Initialization Script
-- This script creates the necessary tables for RestaurantSuite

-- Create Users table (for staff members)
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "Email" TEXT NOT NULL,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Phone" TEXT,
    "PasswordHash" TEXT NOT NULL,
    "RefreshToken" TEXT,
    "RefreshTokenExpiresAt" TEXT,
    "Role" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id"),
    CONSTRAINT "IX_Users_Email" UNIQUE ("Email")
);

-- Create Restaurants table
CREATE TABLE IF NOT EXISTS "Restaurants" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Address" TEXT NOT NULL,
    "Timezone" TEXT NOT NULL,
    "Currency" TEXT NOT NULL,
    "SettingsJson" TEXT
);

-- Create Categories table
CREATE TABLE IF NOT EXISTS "Categories" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "DisplayOrder" INTEGER NOT NULL
);

-- Create MenuItems table
CREATE TABLE IF NOT EXISTS "MenuItems" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Description" TEXT,
    "Price" TEXT NOT NULL,
    "ImageUrl" TEXT,
    "IsAvailable" INTEGER NOT NULL,
    "CategoryId" TEXT NOT NULL,
    CONSTRAINT "FK_MenuItems_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT
);

-- Create Tables table
CREATE TABLE IF NOT EXISTS "Tables" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "TableNumber" TEXT NOT NULL,
    "Capacity" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL,
    CONSTRAINT "IX_Tables_TableNumber" UNIQUE ("TableNumber")
);

-- Create Orders table
CREATE TABLE IF NOT EXISTS "Orders" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "TableId" TEXT NOT NULL,
    "WaiterId" TEXT NOT NULL,
    "GuestId" TEXT,
    "Status" INTEGER NOT NULL,
    "TotalAmount" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT,
    CONSTRAINT "FK_Orders_Tables_TableId" FOREIGN KEY ("TableId") REFERENCES "Tables" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Orders_Users_WaiterId" FOREIGN KEY ("WaiterId") REFERENCES "Users" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Orders_Users_GuestId" FOREIGN KEY ("GuestId") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

-- Create OrderItems table
CREATE TABLE IF NOT EXISTS "OrderItems" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "OrderId" TEXT NOT NULL,
    "MenuItemId" TEXT NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "UnitPrice" TEXT NOT NULL,
    "Subtotal" TEXT NOT NULL,
    "SpecialInstructions" TEXT,
    CONSTRAINT "FK_OrderItems_MenuItems_MenuItemId" FOREIGN KEY ("MenuItemId") REFERENCES "MenuItems" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
);

-- Insert sample staff members for testing
INSERT OR IGNORE INTO "Users" ("Id", "Email", "FirstName", "LastName", "PasswordHash", "Role", "IsActive", "Phone", "CreatedAt")
VALUES 
    ('00000000-0000-0000-0000-000000000001', 'mark@restaurant.com', 'Mark', 'Johnson', 'temp_hash', 0, 1, '+381 11 123 4567', datetime('now')),
    ('00000000-0000-0000-0000-000000000002', 'sarah@restaurant.com', 'Sarah', 'Williams', 'temp_hash', 1, 1, '+381 11 123 4568', datetime('now')),
    ('00000000-0000-0000-0000-000000000003', 'david@restaurant.com', 'David', 'Brown', 'temp_hash', 0, 1, '+381 11 123 4569', datetime('now')),
    ('00000000-0000-0000-0000-000000000004', 'emma@restaurant.com', 'Emma', 'Wilson', 'temp_hash', 2, 1, '+381 11 123 4570', datetime('now'));

-- Verify the data
SELECT 
    "Id",
    "FirstName" || ' ' || "LastName" as "FullName",
    "Email",
    "Phone",
    CASE 
        WHEN "Role" = 0 THEN 'Waiter'
        WHEN "Role" = 1 THEN 'Chef'
        WHEN "Role" = 2 THEN 'Admin'
        ELSE 'Unknown'
    END as "Role",
    "IsActive",
    "CreatedAt"
FROM "Users"
ORDER BY "CreatedAt" DESC;

-- Display success message
SELECT 'SQLite database setup completed successfully!' as "Status";
SELECT 'Staff members table is ready for use!' as "StaffTableStatus";
SELECT 'Sample staff members have been inserted!' as "SampleDataStatus";
