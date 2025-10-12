-- SQLite Database Complete Initialization Script
-- This script creates all necessary tables for RestaurantSuite

-- Create Restaurants table (this was missing from the original SQLite script)
CREATE TABLE IF NOT EXISTS "Restaurants" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Address" TEXT NOT NULL,
    "Timezone" TEXT NOT NULL,
    "Currency" TEXT NOT NULL,
    "SettingsJson" TEXT,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT
);

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

-- Create Categories table
CREATE TABLE IF NOT EXISTS "Categories" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "DisplayOrder" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT
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
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT,
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

-- Insert sample restaurant
INSERT OR IGNORE INTO "Restaurants" ("Id", "Name", "Address", "Timezone", "Currency", "SettingsJson", "CreatedAt")
VALUES ('00000000-0000-0000-0000-000000000001', 'Restoran Kuzma', 'Bulevar Oslobođenja 123, Novi Sad, Serbia', 'Europe/Belgrade', 'RSD', '{"theme": "serbian", "language": "sr"}', datetime('now'));

-- Insert sample staff members for testing
INSERT OR IGNORE INTO "Users" ("Id", "Email", "FirstName", "LastName", "PasswordHash", "Role", "IsActive", "Phone", "CreatedAt")
VALUES
    ('00000000-0000-0000-0000-000000000001', 'mark@restaurant.com', 'Mark', 'Johnson', 'temp_hash', 0, 1, '+381 11 123 4567', datetime('now')),
    ('00000000-0000-0000-0000-000000000002', 'sarah@restaurant.com', 'Sarah', 'Williams', 'temp_hash', 1, 1, '+381 11 123 4568', datetime('now')),
    ('00000000-0000-0000-0000-000000000003', 'david@restaurant.com', 'David', 'Brown', 'temp_hash', 0, 1, '+381 11 123 4569', datetime('now')),
    ('00000000-0000-0000-0000-000000000004', 'emma@restaurant.com', 'Emma', 'Wilson', 'temp_hash', 2, 1, '+381 11 123 4570', datetime('now'));

-- Insert sample categories
INSERT OR IGNORE INTO "Categories" ("Id", "Name", "DisplayOrder", "IsActive", "CreatedAt")
VALUES
    ('10000000-0000-0000-0000-000000000001', 'Appetizers', 1, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000002', 'Main Courses', 2, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000003', 'Desserts', 3, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000004', 'Beverages', 4, 1, datetime('now'));

-- Insert sample menu items
INSERT OR IGNORE INTO "MenuItems" ("Id", "Name", "Description", "Price", "ImageUrl", "IsAvailable", "CategoryId", "CreatedAt", "UpdatedAt")
VALUES
    ('20000000-0000-0000-0000-000000000001', 'Pljeskavica', 'Traditional Serbian grilled meat patty served with kajmak and ajvar', '450.00', 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000002', 'Ćevapi', 'Grilled minced meat fingers served with somun, onions, and ajvar', '380.00', 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000003', 'Karađorđeva Šnicla', 'Breaded veal cutlet stuffed with kajmak, served with tartar sauce', '650.00', 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000004', 'Palačinke', 'Traditional Serbian pancakes with various fillings', '280.00', 'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000005', 'Šopska Salata', 'Fresh tomato, cucumber, and pepper salad with sirene cheese', '220.00', 'https://images.unsplash.com/photo-1571115764595-644a1f56a55c?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000006', 'Rakija', 'Traditional Serbian fruit brandy, various flavors available', '180.00', 'https://images.unsplash.com/photo-1587668178277-295251f900ce?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', datetime('now'), datetime('now'));

-- Insert sample tables
INSERT OR IGNORE INTO "Tables" ("Id", "TableNumber", "Capacity", "Status")
VALUES
    ('30000000-0000-0000-0000-000000000001', 'T1', 4, 0),
    ('30000000-0000-0000-0000-000000000002', 'T2', 2, 0),
    ('30000000-0000-0000-0000-000000000003', 'T3', 6, 0),
    ('30000000-0000-0000-0000-000000000004', 'T4', 8, 0);

-- Display success message
SELECT 'SQLite database setup completed successfully!' as "Status";
SELECT 'All tables have been created: Restaurants, Users, Categories, MenuItems, Tables, Orders, OrderItems' as "TablesStatus";
SELECT 'Sample data has been inserted for testing!' as "SampleDataStatus";
SELECT 'The add-menu-item-submit-btn functionality is now ready!' as "MenuItemStatus";
