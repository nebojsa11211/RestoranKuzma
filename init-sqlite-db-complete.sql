-- SQLite Database Complete Initialization Script
-- This script creates all necessary tables for RestaurantSuite

-- Drop all tables to ensure clean slate
DROP TABLE IF EXISTS "OrderItemCustomizations";
DROP TABLE IF EXISTS "MenuItemIngredients";
DROP TABLE IF EXISTS "OrderItems";
DROP TABLE IF EXISTS "Orders";
DROP TABLE IF EXISTS "Tables";
DROP TABLE IF EXISTS "MenuItems";
DROP TABLE IF EXISTS "Categories";
DROP TABLE IF EXISTS "Users";
DROP TABLE IF EXISTS "Restaurants";

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
    "Email" TEXT NOT NULL UNIQUE,
    "FirstName" TEXT NOT NULL,
    "LastName" TEXT NOT NULL,
    "Phone" TEXT,
    "PasswordHash" TEXT NOT NULL,
    "RefreshToken" TEXT,
    "RefreshTokenExpiresAt" TEXT,
    "Role" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT
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
    "PreparationTimeMinutes" INTEGER,
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

-- Create MenuItemIngredients table
CREATE TABLE IF NOT EXISTS "MenuItemIngredients" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "MenuItemId" TEXT NOT NULL,
    "IngredientName" TEXT NOT NULL,
    "QuantityInGrams" TEXT NOT NULL,
    "IsMainIngredient" INTEGER NOT NULL,
    "DisplayOrder" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_MenuItemIngredients_MenuItems_MenuItemId" FOREIGN KEY ("MenuItemId") REFERENCES "MenuItems" ("Id") ON DELETE CASCADE
);

-- Create OrderItemCustomizations table
CREATE TABLE IF NOT EXISTS "OrderItemCustomizations" (
    "Id" TEXT NOT NULL PRIMARY KEY,
    "OrderItemId" TEXT NOT NULL,
    "IngredientName" TEXT NOT NULL,
    "CustomizationType" INTEGER NOT NULL,
    "Notes" TEXT,
    "CreatedAt" TEXT NOT NULL,
    CONSTRAINT "FK_OrderItemCustomizations_OrderItems_OrderItemId" FOREIGN KEY ("OrderItemId") REFERENCES "OrderItems" ("Id") ON DELETE CASCADE
);

-- Insert sample restaurant
INSERT OR IGNORE INTO "Restaurants" ("Id", "Name", "Address", "Timezone", "Currency", "SettingsJson", "CreatedAt")
VALUES ('00000000-0000-0000-0000-000000000001', 'Restoran Kuzma', 'Bulevar Oslobođenja 123, Novi Sad, Serbia', 'Europe/Belgrade', 'RSD', '{"theme": "serbian", "language": "sr"}', datetime('now'));

-- Insert sample staff members for testing
-- All users have password: password123
INSERT OR IGNORE INTO "Users" ("Id", "Email", "FirstName", "LastName", "PasswordHash", "Role", "IsActive", "Phone", "CreatedAt")
VALUES
    ('00000000-0000-0000-0000-000000000001', 'mark@restaurant.com', 'Mark', 'Johnson', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 0, 1, '+381 11 123 4567', datetime('now')),
    ('00000000-0000-0000-0000-000000000002', 'sarah@restaurant.com', 'Sarah', 'Williams', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 123 4568', datetime('now')),
    ('00000000-0000-0000-0000-000000000003', 'david@restaurant.com', 'David', 'Brown', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 0, 1, '+381 11 123 4569', datetime('now')),
    ('00000000-0000-0000-0000-000000000004', 'emma@restaurant.com', 'Emma', 'Wilson', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 2, 1, '+381 11 123 4570', datetime('now'));

-- Additional Chefs (Role = 2)
INSERT OR IGNORE INTO "Users" ("Id", "Email", "FirstName", "LastName", "PasswordHash", "Role", "IsActive", "Phone", "CreatedAt")
VALUES
    ('00000000-0000-0000-0000-000000000005', 'milan.petrović@restaurant.com', 'Milan', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 2, 1, '+381 11 123 4571', datetime('now')),
    ('00000000-0000-0000-0000-000000000006', 'dragan.jovanović@restaurant.com', 'Dragan', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 2, 1, '+381 11 124 4572', datetime('now')),
    ('00000000-0000-0000-0000-000000000007', 'nikola.nikolić@restaurant.com', 'Nikola', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 2, 1, '+381 11 125 4573', datetime('now'));

-- Additional Waiters (Role = 1)
INSERT OR IGNORE INTO "Users" ("Id", "Email", "FirstName", "LastName", "PasswordHash", "Role", "IsActive", "Phone", "CreatedAt")
VALUES
    ('00000000-0000-0000-0000-000000000008', 'ana.stojanović@restaurant.com', 'Ana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 200 4600', datetime('now')),
    ('00000000-0000-0000-0000-000000000009', 'marko.ilić@restaurant.com', 'Marko', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 201 4601', datetime('now')),
    ('00000000-0000-0000-0000-000000000010', 'jovana.pavlović@restaurant.com', 'Jovana', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 202 4602', datetime('now')),
    ('00000000-0000-0000-0000-000000000011', 'stefan.marković@restaurant.com', 'Stefan', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 203 4603', datetime('now')),
    ('00000000-0000-0000-0000-000000000012', 'milica.đorđević@restaurant.com', 'Milica', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 204 4604', datetime('now')),
    ('00000000-0000-0000-0000-000000000013', 'aleksandar.popović@restaurant.com', 'Aleksandar', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 205 4605', datetime('now')),
    ('00000000-0000-0000-0000-000000000014', 'jelena.nikolić@restaurant.com', 'Jelena', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 206 4606', datetime('now')),
    ('00000000-0000-0000-0000-000000000015', 'nemanja.simić@restaurant.com', 'Nemanja', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 207 4607', datetime('now')),
    ('00000000-0000-0000-0000-000000000016', 'tijana.stanković@restaurant.com', 'Tijana', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 208 4608', datetime('now')),
    ('00000000-0000-0000-0000-000000000017', 'luka.radovanović@restaurant.com', 'Luka', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 209 4609', datetime('now')),
    ('00000000-0000-0000-0000-000000000018', 'sara.vasić@restaurant.com', 'Sara', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 210 4610', datetime('now')),
    ('00000000-0000-0000-0000-000000000019', 'filip.antić@restaurant.com', 'Filip', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 211 4611', datetime('now')),
    ('00000000-0000-0000-0000-000000000020', 'katarina.milošević@restaurant.com', 'Katarina', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 212 4612', datetime('now')),
    ('00000000-0000-0000-0000-000000000021', 'miloš.dimitrijević@restaurant.com', 'Miloš', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 213 4613', datetime('now')),
    ('00000000-0000-0000-0000-000000000022', 'teodora.kovačević@restaurant.com', 'Teodora', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 1, 1, '+381 11 214 4614', datetime('now'));

-- Guest Users (Role = 3) - 300 unique guests
INSERT OR IGNORE INTO "Users" ("Id", "Email", "FirstName", "LastName", "PasswordHash", "Role", "IsActive", "CreatedAt")
VALUES
    ('00000000-0000-0000-0000-000000000023', 'petar.petrović23@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000024', 'ana.jovanović24@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000025', 'marko.marković25@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000026', 'milica.nikolić26@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000027', 'stefan.đorđević27@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000028', 'jovana.stojanović28@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000029', 'nikola.ilić29@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000030', 'jelena.pavlović30@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000031', 'luka.popović31@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000032', 'sara.simić32@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000033', 'milan.radovanović33@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000034', 'katarina.stanković34@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000035', 'aleksandar.vasić35@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000036', 'tijana.antić36@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000037', 'filip.milošević37@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000038', 'teodora.dimitrijević38@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000039', 'miloš.kovačević39@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000040', 'sofija.mladenović40@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000041', 'vladimir.kostić41@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000042', 'anđela.todorović42@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000043', 'dušan.lazarević43@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000044', 'mila.stojković44@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000045', 'đorđe.đukić45@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000046', 'emilija.vuković46@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000047', 'pavle.tošić47@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000048', 'petar.petrović48@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000049', 'ana.jovanović49@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000050', 'marko.marković50@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000051', 'milica.nikolić51@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000052', 'stefan.đorđević52@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000053', 'jovana.stojanović53@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000054', 'nikola.ilić54@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000055', 'jelena.pavlović55@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000056', 'luka.popović56@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000057', 'sara.simić57@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000058', 'milan.radovanović58@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000059', 'katarina.stanković59@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000060', 'aleksandar.vasić60@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000061', 'tijana.antić61@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000062', 'filip.milošević62@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000063', 'teodora.dimitrijević63@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000064', 'miloš.kovačević64@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000065', 'sofija.mladenović65@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000066', 'vladimir.kostić66@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000067', 'anđela.todorović67@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000068', 'dušan.lazarević68@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000069', 'mila.stojković69@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000070', 'đorđe.đukić70@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000071', 'emilija.vuković71@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000072', 'pavle.tošić72@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000073', 'petar.petrović73@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000074', 'ana.jovanović74@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000075', 'marko.marković75@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000076', 'milica.nikolić76@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000077', 'stefan.đorđević77@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000078', 'jovana.stojanović78@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000079', 'nikola.ilić79@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000080', 'jelena.pavlović80@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000081', 'luka.popović81@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000082', 'sara.simić82@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000083', 'milan.radovanović83@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000084', 'katarina.stanković84@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000085', 'aleksandar.vasić85@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000086', 'tijana.antić86@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000087', 'filip.milošević87@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000088', 'teodora.dimitrijević88@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000089', 'miloš.kovačević89@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000090', 'sofija.mladenović90@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000091', 'vladimir.kostić91@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000092', 'anđela.todorović92@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000093', 'dušan.lazarević93@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000094', 'mila.stojković94@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000095', 'đorđe.đukić95@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000096', 'emilija.vuković96@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000097', 'pavle.tošić97@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000098', 'petar.petrović98@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000099', 'ana.jovanović99@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000100', 'marko.marković100@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000101', 'milica.nikolić101@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000102', 'stefan.đorđević102@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000103', 'jovana.stojanović103@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000104', 'nikola.ilić104@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000105', 'jelena.pavlović105@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000106', 'luka.popović106@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000107', 'sara.simić107@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000108', 'milan.radovanović108@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000109', 'katarina.stanković109@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000110', 'aleksandar.vasić110@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000111', 'tijana.antić111@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000112', 'filip.milošević112@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000113', 'teodora.dimitrijević113@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000114', 'miloš.kovačević114@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000115', 'sofija.mladenović115@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000116', 'vladimir.kostić116@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000117', 'anđela.todorović117@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000118', 'dušan.lazarević118@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000119', 'mila.stojković119@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000120', 'đorđe.đukić120@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000121', 'emilija.vuković121@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000122', 'pavle.tošić122@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000123', 'petar.petrović123@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000124', 'ana.jovanović124@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000125', 'marko.marković125@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000126', 'milica.nikolić126@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000127', 'stefan.đorđević127@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000128', 'jovana.stojanović128@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000129', 'nikola.ilić129@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000130', 'jelena.pavlović130@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000131', 'luka.popović131@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000132', 'sara.simić132@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000133', 'milan.radovanović133@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000134', 'katarina.stanković134@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000135', 'aleksandar.vasić135@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000136', 'tijana.antić136@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000137', 'filip.milošević137@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000138', 'teodora.dimitrijević138@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000139', 'miloš.kovačević139@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000140', 'sofija.mladenović140@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000141', 'vladimir.kostić141@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000142', 'anđela.todorović142@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000143', 'dušan.lazarević143@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000144', 'mila.stojković144@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000145', 'đorđe.đukić145@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000146', 'emilija.vuković146@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000147', 'pavle.tošić147@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000148', 'petar.petrović148@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000149', 'ana.jovanović149@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000150', 'marko.marković150@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000151', 'milica.nikolić151@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000152', 'stefan.đorđević152@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000153', 'jovana.stojanović153@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000154', 'nikola.ilić154@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000155', 'jelena.pavlović155@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000156', 'luka.popović156@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000157', 'sara.simić157@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000158', 'milan.radovanović158@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000159', 'katarina.stanković159@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000160', 'aleksandar.vasić160@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000161', 'tijana.antić161@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000162', 'filip.milošević162@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000163', 'teodora.dimitrijević163@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000164', 'miloš.kovačević164@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000165', 'sofija.mladenović165@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000166', 'vladimir.kostić166@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000167', 'anđela.todorović167@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000168', 'dušan.lazarević168@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000169', 'mila.stojković169@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000170', 'đorđe.đukić170@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000171', 'emilija.vuković171@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000172', 'pavle.tošić172@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000173', 'petar.petrović173@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000174', 'ana.jovanović174@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000175', 'marko.marković175@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000176', 'milica.nikolić176@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000177', 'stefan.đorđević177@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000178', 'jovana.stojanović178@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000179', 'nikola.ilić179@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000180', 'jelena.pavlović180@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000181', 'luka.popović181@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000182', 'sara.simić182@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000183', 'milan.radovanović183@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000184', 'katarina.stanković184@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000185', 'aleksandar.vasić185@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000186', 'tijana.antić186@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000187', 'filip.milošević187@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000188', 'teodora.dimitrijević188@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000189', 'miloš.kovačević189@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000190', 'sofija.mladenović190@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000191', 'vladimir.kostić191@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000192', 'anđela.todorović192@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000193', 'dušan.lazarević193@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000194', 'mila.stojković194@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000195', 'đorđe.đukić195@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000196', 'emilija.vuković196@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000197', 'pavle.tošić197@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000198', 'petar.petrović198@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000199', 'ana.jovanović199@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000200', 'marko.marković200@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000201', 'milica.nikolić201@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000202', 'stefan.đorđević202@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000203', 'jovana.stojanović203@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000204', 'nikola.ilić204@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000205', 'jelena.pavlović205@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000206', 'luka.popović206@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000207', 'sara.simić207@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000208', 'milan.radovanović208@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000209', 'katarina.stanković209@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000210', 'aleksandar.vasić210@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000211', 'tijana.antić211@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000212', 'filip.milošević212@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000213', 'teodora.dimitrijević213@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000214', 'miloš.kovačević214@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000215', 'sofija.mladenović215@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000216', 'vladimir.kostić216@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000217', 'anđela.todorović217@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000218', 'dušan.lazarević218@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000219', 'mila.stojković219@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000220', 'đorđe.đukić220@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000221', 'emilija.vuković221@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000222', 'pavle.tošić222@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000223', 'petar.petrović223@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000224', 'ana.jovanović224@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000225', 'marko.marković225@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000226', 'milica.nikolić226@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000227', 'stefan.đorđević227@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000228', 'jovana.stojanović228@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000229', 'nikola.ilić229@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000230', 'jelena.pavlović230@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000231', 'luka.popović231@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000232', 'sara.simić232@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000233', 'milan.radovanović233@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000234', 'katarina.stanković234@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000235', 'aleksandar.vasić235@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000236', 'tijana.antić236@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000237', 'filip.milošević237@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000238', 'teodora.dimitrijević238@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000239', 'miloš.kovačević239@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000240', 'sofija.mladenović240@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000241', 'vladimir.kostić241@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000242', 'anđela.todorović242@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000243', 'dušan.lazarević243@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000244', 'mila.stojković244@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000245', 'đorđe.đukić245@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000246', 'emilija.vuković246@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000247', 'pavle.tošić247@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000248', 'petar.petrović248@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000249', 'ana.jovanović249@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000250', 'marko.marković250@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000251', 'milica.nikolić251@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000252', 'stefan.đorđević252@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000253', 'jovana.stojanović253@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000254', 'nikola.ilić254@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000255', 'jelena.pavlović255@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000256', 'luka.popović256@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000257', 'sara.simić257@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000258', 'milan.radovanović258@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000259', 'katarina.stanković259@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000260', 'aleksandar.vasić260@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000261', 'tijana.antić261@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000262', 'filip.milošević262@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000263', 'teodora.dimitrijević263@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000264', 'miloš.kovačević264@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000265', 'sofija.mladenović265@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000266', 'vladimir.kostić266@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000267', 'anđela.todorović267@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000268', 'dušan.lazarević268@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000269', 'mila.stojković269@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000270', 'đorđe.đukić270@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000271', 'emilija.vuković271@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000272', 'pavle.tošić272@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000273', 'petar.petrović273@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000274', 'ana.jovanović274@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000275', 'marko.marković275@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000276', 'milica.nikolić276@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000277', 'stefan.đorđević277@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000278', 'jovana.stojanović278@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000279', 'nikola.ilić279@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000280', 'jelena.pavlović280@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000281', 'luka.popović281@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000282', 'sara.simić282@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000283', 'milan.radovanović283@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000284', 'katarina.stanković284@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000285', 'aleksandar.vasić285@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000286', 'tijana.antić286@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000287', 'filip.milošević287@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000288', 'teodora.dimitrijević288@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000289', 'miloš.kovačević289@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000290', 'sofija.mladenović290@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000291', 'vladimir.kostić291@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000292', 'anđela.todorović292@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000293', 'dušan.lazarević293@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000294', 'mila.stojković294@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000295', 'đorđe.đukić295@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000296', 'emilija.vuković296@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000297', 'pavle.tošić297@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000298', 'petar.petrović298@guest.com', 'Petar', 'Petrović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000299', 'ana.jovanović299@guest.com', 'Ana', 'Jovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000300', 'marko.marković300@guest.com', 'Marko', 'Marković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000301', 'milica.nikolić301@guest.com', 'Milica', 'Nikolić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000302', 'stefan.đorđević302@guest.com', 'Stefan', 'Đorđević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000303', 'jovana.stojanović303@guest.com', 'Jovana', 'Stojanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000304', 'nikola.ilić304@guest.com', 'Nikola', 'Ilić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000305', 'jelena.pavlović305@guest.com', 'Jelena', 'Pavlović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000306', 'luka.popović306@guest.com', 'Luka', 'Popović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000307', 'sara.simić307@guest.com', 'Sara', 'Simić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000308', 'milan.radovanović308@guest.com', 'Milan', 'Radovanović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000309', 'katarina.stanković309@guest.com', 'Katarina', 'Stanković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000310', 'aleksandar.vasić310@guest.com', 'Aleksandar', 'Vasić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000311', 'tijana.antić311@guest.com', 'Tijana', 'Antić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000312', 'filip.milošević312@guest.com', 'Filip', 'Milošević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000313', 'teodora.dimitrijević313@guest.com', 'Teodora', 'Dimitrijević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000314', 'miloš.kovačević314@guest.com', 'Miloš', 'Kovačević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000315', 'sofija.mladenović315@guest.com', 'Sofija', 'Mladenović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000316', 'vladimir.kostić316@guest.com', 'Vladimir', 'Kostić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000317', 'anđela.todorović317@guest.com', 'Anđela', 'Todorović', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000318', 'dušan.lazarević318@guest.com', 'Dušan', 'Lazarević', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000319', 'mila.stojković319@guest.com', 'Mila', 'Stojković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000320', 'đorđe.đukić320@guest.com', 'Đorđe', 'Đukić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000321', 'emilija.vuković321@guest.com', 'Emilija', 'Vuković', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now')),
    ('00000000-0000-0000-0000-000000000322', 'pavle.tošić322@guest.com', 'Pavle', 'Tošić', '75K3eLr+dx6JJFuJ7LwIpEpOFmwGZZkRiB84PURz6U8=', 3, 1, datetime('now'));
-- Insert sample categories
INSERT OR IGNORE INTO "Categories" ("Id", "Name", "DisplayOrder", "IsActive", "CreatedAt")
VALUES
    ('10000000-0000-0000-0000-000000000001', 'Appetizers', 1, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000002', 'Salads', 2, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000003', 'Main Courses', 3, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000004', 'Pizzas', 4, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000005', 'Soups', 5, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000006', 'Desserts', 6, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000007', 'Hot Drinks', 7, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000008', 'Cold Drinks', 8, 1, datetime('now')),
    ('10000000-0000-0000-0000-000000000009', 'Alcoholic Beverages', 9, 1, datetime('now'));

-- Insert sample menu items with PreparationTimeMinutes
INSERT OR IGNORE INTO "MenuItems" ("Id", "Name", "Description", "Price", "ImageUrl", "IsAvailable", "CategoryId", "PreparationTimeMinutes", "CreatedAt", "UpdatedAt")
VALUES
    -- Main Courses (Category: 10000000-0000-0000-0000-000000000003)
    ('20000000-0000-0000-0000-000000000001', 'Pljeskavica', 'Traditional Serbian grilled meat patty served with kajmak and ajvar', '450.00', 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 15, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000002', 'Ćevapi', 'Grilled minced meat fingers served with somun, onions, and ajvar', '380.00', 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 12, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000003', 'Karađorđeva Šnicla', 'Breaded veal cutlet stuffed with kajmak, served with tartar sauce', '650.00', 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 20, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000007', 'Sarma', 'Cabbage rolls stuffed with minced meat and rice, served with sour cream', '520.00', 'https://images.unsplash.com/photo-1574484284002-952d92456975?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 35, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000008', 'Prebranac', 'Baked beans with onions, paprika and smoked meat', '420.00', 'https://images.unsplash.com/photo-1615485500834-bc10199bc727?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 25, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000009', 'Punjena Paprika', 'Bell peppers stuffed with meat and rice in tomato sauce', '480.00', 'https://images.unsplash.com/photo-1625943553852-781c6dd46faa?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 30, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000010', 'Vesalica', 'Mixed grill platter with pljeskavica, ćevapi, and sausage', '780.00', 'https://images.unsplash.com/photo-1544025162-d76694265947?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 18, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000011', 'Ražnjići', 'Grilled pork skewers with vegetables', '550.00', 'https://images.unsplash.com/photo-1603360946369-dc9bb6258143?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 16, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000012', 'Gulaš', 'Traditional beef goulash with paprika and vegetables', '580.00', 'https://images.unsplash.com/photo-1534939561126-855b8675edd7?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 40, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000013', 'Bečka Šnicla', 'Breaded and fried veal cutlet, Vienna style', '620.00', 'https://images.unsplash.com/photo-1600891964092-4316c288032e?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 15, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000014', 'Roštilj Mešano', 'Mixed grill selection for two people', '1450.00', 'https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 22, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000015', 'Leskovačka Mućkalica', 'Spicy meat stew with peppers and tomatoes', '560.00', 'https://images.unsplash.com/photo-1547592166-23ac45744acd?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 28, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000016', 'Punjene Tikvice', 'Stuffed zucchini with meat and rice', '460.00', 'https://images.unsplash.com/photo-1612874742237-6526221588e3?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 30, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000018', 'Vešalica Pork', 'Pork chop stuffed with kajmak and prosciutto', '640.00', 'https://images.unsplash.com/photo-1504973960431-1c467e159aa4?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 18, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000019', 'Ćufte', 'Serbian meatballs in rich tomato sauce', '490.00', 'https://images.unsplash.com/photo-1529042410759-befb1204b468?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 22, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000020', 'Pork Karadjordje', 'Rolled pork with prosciutto and cheese, breaded and fried', '680.00', 'https://images.unsplash.com/photo-1432139555190-58524dae6a55?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 20, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000021', 'Jadranska Plata', 'Seafood platter with grilled fish and calamari', '950.00', 'https://images.unsplash.com/photo-1559737558-2f5a13e2e7e3?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 24, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000022', 'Pasulj', 'Traditional bean stew with smoked meat', '440.00', 'https://images.unsplash.com/photo-1587740908075-9e5b2f91e43b?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 35, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000023', 'Chicken Steak', 'Grilled chicken breast with mushroom sauce', '580.00', 'https://images.unsplash.com/photo-1598103442097-8b74394b95c6?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 16, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000024', 'Lamb Ribs', 'Grilled lamb ribs with herb marinade', '890.00', 'https://images.unsplash.com/photo-1544025162-d76694265947?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000003', 25, datetime('now'), datetime('now')),

    -- Appetizers (Category: 10000000-0000-0000-0000-000000000001)
    ('20000000-0000-0000-0000-000000000026', 'Gibanica', 'Savory cheese pie with layers of phyllo dough', '320.00', 'https://images.unsplash.com/photo-1562967914-608f82629710?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 8, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000027', 'Urnebes', 'Spicy cheese spread with hot peppers', '180.00', 'https://images.unsplash.com/photo-1618164436241-4473940d1f5c?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 3, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000028', 'Ajvar Domaći', 'Homemade red pepper relish', '150.00', 'https://images.unsplash.com/photo-1571942676516-bcab84649e44?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 2, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000029', 'Kajmak', 'Creamy dairy product, traditional Serbian spread', '200.00', 'https://images.unsplash.com/photo-1452195100486-9cc805987862?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 2, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000030', 'Pršuta', 'Dried prosciutto with cheese selection', '480.00', 'https://images.unsplash.com/photo-1595777216528-85c264ad3f4c?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 5, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000034', 'Pečurke na Žaru', 'Grilled mushrooms with garlic and herbs', '320.00', 'https://images.unsplash.com/photo-1621270607707-fe3e2a3a0a0d?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 10, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000035', 'Kiseli Krastavci', 'Pickled cucumbers, traditional style', '120.00', 'https://images.unsplash.com/photo-1589621316382-008455b857cd?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 2, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000036', 'Kiseli Kupus', 'Pickled cabbage with carrots', '120.00', 'https://images.unsplash.com/photo-1607532941433-304659e8198d?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 2, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000037', 'Meze Plata', 'Appetizer platter with various Serbian delicacies', '650.00', 'https://images.unsplash.com/photo-1562059390-a761a084768e?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 10, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000038', 'Pečena Paprika', 'Roasted red peppers with garlic', '220.00', 'https://images.unsplash.com/photo-1563865436874-9aef32095fad?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 8, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000087', 'Cheese Platter', 'Selection of Serbian cheeses with honey and walnuts', '550.00', 'https://images.unsplash.com/photo-1486297678162-eb2a19b0a32d?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 5, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000088', 'Pogačice', 'Mini savory pastries with cheese', '240.00', 'https://images.unsplash.com/photo-1509440159596-0249088772ff?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000001', 6, datetime('now'), datetime('now')),

    -- Salads (Category: 10000000-0000-0000-0000-000000000002)
    ('20000000-0000-0000-0000-000000000005', 'Šopska Salata', 'Fresh tomato, cucumber, and pepper salad with sirene cheese', '220.00', 'https://images.unsplash.com/photo-1571115764595-644a1f56a55c?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', 5, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000025', 'Srpska Salata', 'Chopped tomatoes, onions, and hot peppers with olive oil', '200.00', 'https://images.unsplash.com/photo-1546069901-efd447dfea7a?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', 5, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000031', 'Mešana Salata', 'Mixed seasonal salad with house dressing', '240.00', 'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', 5, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000032', 'Grčka Salata', 'Greek salad with feta cheese and olives', '280.00', 'https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', 6, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000033', 'Kupus Salata', 'Cabbage salad with carrots and vinegar dressing', '180.00', 'https://images.unsplash.com/photo-1607532941433-304659e8198d?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', 4, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000089', 'Caesar Salata', 'Caesar salad with grilled chicken, croutons, parmesan', '320.00', 'https://images.unsplash.com/photo-1550304943-4f24f54ddde9?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', 7, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000090', 'Pileća Salata', 'Chicken salad with mixed greens and vinaigrette', '310.00', 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', 6, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000091', 'Tuna Salata', 'Tuna salad with beans, onions, and olive oil', '290.00', 'https://images.unsplash.com/photo-1505253716362-afaea1d3d1af?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000002', 5, datetime('now'), datetime('now')),

    -- Pizzas (Category: 10000000-0000-0000-0000-000000000004)
    ('20000000-0000-0000-0000-000000000067', 'Margherita', 'Classic pizza with tomato sauce, mozzarella and fresh basil', '580.00', 'https://images.unsplash.com/photo-1604068549290-dea0e4a305ca?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 15, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000068', 'Capricciosa', 'Tomato sauce, mozzarella, ham, mushrooms, artichokes, olives', '720.00', 'https://images.unsplash.com/photo-1571997478779-2adcbbe9ab2f?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 18, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000069', 'Quattro Formaggi', 'Four cheese pizza with mozzarella, gorgonzola, parmesan, fontina', '780.00', 'https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 16, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000070', 'Diavola', 'Spicy pizza with tomato sauce, mozzarella, spicy salami, hot peppers', '690.00', 'https://images.unsplash.com/photo-1628840042765-356cda07504e?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 17, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000071', 'Prosciutto e Funghi', 'Ham and mushroom pizza with mozzarella and tomato sauce', '710.00', 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 17, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000072', 'Quattro Stagioni', 'Four seasons pizza with ham, mushrooms, artichokes, and olives in quarters', '750.00', 'https://images.unsplash.com/photo-1574071318508-1cdbab80d002?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 19, datetime('now'), datetime('now')),

    -- Soups (Category: 10000000-0000-0000-0000-000000000005)
    ('20000000-0000-0000-0000-000000000017', 'Teleća Čorba sa Rezancima', 'Veal soup with homemade noodles', '380.00', 'https://images.unsplash.com/photo-1547592180-85f173990554?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000005', 25, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000092', 'Pileća Čorba', 'Chicken soup with vegetables', '320.00', 'https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000005', 20, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000093', 'Riblja Čorba', 'Fish soup with mixed vegetables', '420.00', 'https://images.unsplash.com/photo-1569718212165-3a8278d5f624?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000005', 25, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000094', 'Supa od Povrća', 'Vegetable soup with seasonal vegetables', '280.00', 'https://images.unsplash.com/photo-1547592166-23ac45744acd?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000005', 15, datetime('now'), datetime('now')),

    -- Desserts (Category: 10000000-0000-0000-0000-000000000006)
    ('20000000-0000-0000-0000-000000000004', 'Palačinke', 'Traditional Serbian pancakes with various fillings', '280.00', 'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 10, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000039', 'Baklava', 'Sweet pastry with nuts and honey syrup', '320.00', 'https://images.unsplash.com/photo-1598110750624-207050c4f28c?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 5, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000040', 'Tulumbe', 'Fried dough soaked in sweet syrup', '260.00', 'https://images.unsplash.com/photo-1586040140378-b5d56a39ebdb?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 8, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000041', 'Krempita', 'Custard cream cake with puff pastry', '290.00', 'https://images.unsplash.com/photo-1565958011703-44f9829ba187?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 6, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000042', 'Šnenokle', 'Floating islands - meringue in vanilla custard', '310.00', 'https://images.unsplash.com/photo-1488477181946-6428a0291777?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 15, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000043', 'Krofne', 'Serbian donuts with jam filling', '80.00', 'https://images.unsplash.com/photo-1551024506-0bccd828d307?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 4, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000044', 'Sladoled (3 kugle)', 'Three scoops of ice cream, various flavors', '250.00', 'https://images.unsplash.com/photo-1563805042-7684c019e1cb?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 3, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000045', 'Štrudla od Jabuka', 'Apple strudel with cinnamon', '280.00', 'https://images.unsplash.com/photo-1574085733277-851d9d856a3a?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 7, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000046', 'Sladoled Sunda', 'Ice cream sundae with chocolate sauce and whipped cream', '320.00', 'https://images.unsplash.com/photo-1563729784474-d77dbb933a9e?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 5, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000047', 'Torta Reforma', 'Chocolate and walnut layer cake', '340.00', 'https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 6, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000048', 'Čokoladna Torta', 'Rich chocolate cake', '350.00', 'https://images.unsplash.com/photo-1606313564200-e75d5e30476c?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 6, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000049', 'Medenjaci', 'Traditional honey cookies', '220.00', 'https://images.unsplash.com/photo-1558961363-fa8fdf82db35?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 4, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000050', 'Voćna Salata', 'Fresh fruit salad', '280.00', 'https://images.unsplash.com/photo-1564093497595-593b96d80180?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000006', 5, datetime('now'), datetime('now')),

    -- Hot Drinks (Category: 10000000-0000-0000-0000-000000000007)
    ('20000000-0000-0000-0000-000000000051', 'Espresso', 'Italian style espresso coffee', '100.00', 'https://images.unsplash.com/photo-1510591509098-f4fdc6d0ff04?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000007', 2, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000052', 'Domaća Kafa', 'Traditional Serbian coffee', '90.00', 'https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000007', 3, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000053', 'Cappuccino', 'Espresso with steamed milk and foam', '150.00', 'https://images.unsplash.com/photo-1517487881594-2787fef5ebf7?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000007', 3, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000095', 'Latte Macchiato', 'Layered espresso with steamed milk', '170.00', 'https://images.unsplash.com/photo-1517487881594-2787fef5ebf7?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000007', 3, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000096', 'Hot Chocolate', 'Rich hot chocolate with whipped cream', '180.00', 'https://images.unsplash.com/photo-1542990253-0d0f5be5f0ed?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000007', 4, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000097', 'Čaj (Tea)', 'Selection of herbal and black teas', '120.00', 'https://images.unsplash.com/photo-1576092768241-dec231879fc3?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000007', 3, datetime('now'), datetime('now')),

    -- Cold Drinks (Category: 10000000-0000-0000-0000-000000000008)
    ('20000000-0000-0000-0000-000000000054', 'Coca Cola', 'Classic Coca Cola 0.33l', '150.00', 'https://images.unsplash.com/photo-1554866585-cd94860890b7?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000055', 'Fanta', 'Orange flavored soft drink 0.33l', '150.00', 'https://images.unsplash.com/photo-1624517452488-04e1e23c4b58?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000056', 'Mineralna Voda', 'Mineral water 0.5l', '100.00', 'https://images.unsplash.com/photo-1548839140-29a749e1cf4d?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000061', 'Cedevita', 'Vitamin drink mix with water', '120.00', 'https://images.unsplash.com/photo-1622597467836-f3285f2131b8?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 2, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000064', 'Fresh Orange Juice', 'Freshly squeezed orange juice', '280.00', 'https://images.unsplash.com/photo-1600271886742-f049cd451bba?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 4, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000065', 'Ledeni Čaj', 'Iced tea, lemon or peach', '150.00', 'https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000066', 'Kisela Voda', 'Sparkling mineral water 0.33l', '120.00', 'https://images.unsplash.com/photo-1585310991536-b7d6f8223bf6?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000098', 'Sprite', 'Lemon-lime flavored soft drink 0.33l', '150.00', 'https://images.unsplash.com/photo-1625772452859-1c03d5bf1137?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000099', 'Schweppes Tonic', 'Tonic water 0.25l', '150.00', 'https://images.unsplash.com/photo-1513558161293-cdaf765ed2fd?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000100', 'Fresh Lemonade', 'Homemade lemonade with mint', '250.00', 'https://images.unsplash.com/photo-1523677011781-c91d1bbe2f8d?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 5, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000101', 'Cedjeni Sok od Grejpfruta', 'Fresh grapefruit juice', '290.00', 'https://images.unsplash.com/photo-1600271886742-f049cd451bba?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000008', 4, datetime('now'), datetime('now')),

    -- Alcoholic Beverages (Category: 10000000-0000-0000-0000-000000000009)
    ('20000000-0000-0000-0000-000000000006', 'Rakija', 'Traditional Serbian fruit brandy, various flavors available', '180.00', 'https://images.unsplash.com/photo-1587668178277-295251f900ce?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000062', 'Šljivovica', 'Plum brandy 0.05l', '200.00', 'https://images.unsplash.com/photo-1569529465841-dfecdab7503b?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000063', 'Loza', 'Grape brandy 0.05l', '180.00', 'https://images.unsplash.com/photo-1606764965570-c4bf08ab0b8b?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000057', 'Jelen Pivo', 'Serbian lager beer 0.5l', '220.00', 'https://images.unsplash.com/photo-1608270586620-248524c67de9?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000058', 'Zaječarsko Pivo', 'Serbian pale lager 0.5l', '220.00', 'https://images.unsplash.com/photo-1535958636474-b021ee887b13?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000059', 'Vranac Vino', 'Red wine from Montenegro, glass', '280.00', 'https://images.unsplash.com/photo-1510812431401-41d2bd2722f3?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000060', 'Chardonnay', 'White wine, glass', '260.00', 'https://images.unsplash.com/photo-1474552226712-ac0f0961a954?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000102', 'Heineken', 'Dutch premium lager 0.33l', '250.00', 'https://images.unsplash.com/photo-1608270586620-248524c67de9?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000103', 'Merlot', 'Red wine, glass', '290.00', 'https://images.unsplash.com/photo-1510812431401-41d2bd2722f3?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000104', 'Prosecco', 'Italian sparkling wine, glass', '320.00', 'https://images.unsplash.com/photo-1547595628-c61a29f496f0?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000105', 'Vodka', 'Premium vodka 0.05l', '250.00', 'https://images.unsplash.com/photo-1616484426072-72b0748a8c9a?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000106', 'Gin & Tonic', 'Gin with tonic water and lime', '350.00', 'https://images.unsplash.com/photo-1551538827-9c037cb4f32a?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 3, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000107', 'Whiskey', 'Premium whiskey 0.05l', '380.00', 'https://images.unsplash.com/photo-1527281400683-1aae777175f8?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 1, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000108', 'Mojito', 'Rum cocktail with mint, lime, and soda', '420.00', 'https://images.unsplash.com/photo-1551024506-0bccd828d307?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 5, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000109', 'Piña Colada', 'Rum cocktail with coconut and pineapple', '450.00', 'https://images.unsplash.com/photo-1541167760496-1628856ab772?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000009', 5, datetime('now'), datetime('now')),

    -- More Pizzas (Category: 10000000-0000-0000-0000-000000000004) - continued from above
    ('20000000-0000-0000-0000-000000000073', 'Vegetariana', 'Vegetarian pizza with grilled vegetables, mozzarella, tomato sauce', '650.00', 'https://images.unsplash.com/photo-1511689660979-10d2b1aada49?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 16, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000074', 'Pepperoni', 'Classic pepperoni pizza with extra mozzarella and tomato sauce', '680.00', 'https://images.unsplash.com/photo-1628840042765-356cda07504e?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 16, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000075', 'Marinara', 'Simple pizza with tomato sauce, garlic, oregano, and olive oil', '490.00', 'https://images.unsplash.com/photo-1595854341625-f33ee10dbf94?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 14, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000076', 'Tonno e Cipolla', 'Tuna and onion pizza with mozzarella, tomato sauce, and capers', '720.00', 'https://images.unsplash.com/photo-1593560708920-61dd98c46a4e?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 16, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000077', 'Frutti di Mare', 'Seafood pizza with shrimp, mussels, calamari, and garlic', '890.00', 'https://images.unsplash.com/photo-1576458088443-04a19c4a87bd?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 20, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000078', 'Bianca', 'White pizza with mozzarella, ricotta, garlic, and olive oil', '640.00', 'https://images.unsplash.com/photo-1571407970349-bc81e7e96c47?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 15, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000079', 'Prosciutto Crudo', 'Pizza with tomato sauce, mozzarella, prosciutto crudo, arugula, parmesan', '820.00', 'https://images.unsplash.com/photo-1595708894623-e4a05f6b1be9?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 17, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000080', 'Napoletana', 'Neapolitan pizza with tomato sauce, mozzarella, anchovies, capers, oregano', '690.00', 'https://images.unsplash.com/photo-1600028068383-ea11a7a101f3?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 16, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000081', 'BBQ Chicken', 'Pizza with BBQ sauce, grilled chicken, red onion, mozzarella, cilantro', '760.00', 'https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 18, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000082', 'Funghi', 'Mushroom pizza with mixed mushrooms, mozzarella, garlic, truffle oil', '730.00', 'https://images.unsplash.com/photo-1588315029754-2dd089d39a1a?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 17, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000083', 'Calzone', 'Folded pizza with ham, mozzarella, mushrooms, tomato sauce inside', '720.00', 'https://images.unsplash.com/photo-1617343267882-ce675a31c6a5?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 19, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000084', 'Mexicana', 'Spicy Mexican pizza with beef, jalapeños, corn, beans, cheddar, sour cream', '780.00', 'https://images.unsplash.com/photo-1601924582970-9238bcb495d9?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 18, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000085', 'Salami', 'Classic salami pizza with tomato sauce, mozzarella, Italian salami', '670.00', 'https://images.unsplash.com/photo-1594007654729-407eedc4be65?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 16, datetime('now'), datetime('now')),
    ('20000000-0000-0000-0000-000000000086', 'Carbonara', 'White pizza with cream, bacon, egg yolk, parmesan, black pepper', '750.00', 'https://images.unsplash.com/photo-1571997478779-2adcbbe9ab2f?w=400&h=200&fit=crop', 1, '10000000-0000-0000-0000-000000000004', 17, datetime('now'), datetime('now'));

-- Insert sample ingredients for menu items
-- Pljeskavica ingredients
INSERT OR IGNORE INTO "MenuItemIngredients" ("Id", "MenuItemId", "IngredientName", "QuantityInGrams", "IsMainIngredient", "DisplayOrder", "CreatedAt")
VALUES
    ('40000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001', 'Ground Beef', '200.00', 1, 1, datetime('now')),
    ('40000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000001', 'Ground Pork', '100.00', 1, 2, datetime('now')),
    ('40000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000001', 'Kajmak', '50.00', 1, 3, datetime('now')),
    ('40000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000001', 'Ajvar', '30.00', 1, 4, datetime('now')),
    ('40000000-0000-0000-0000-000000000005', '20000000-0000-0000-0000-000000000001', 'Onion', '20.00', 0, 5, datetime('now')),
    ('40000000-0000-0000-0000-000000000006', '20000000-0000-0000-0000-000000000001', 'Garlic', '10.00', 0, 6, datetime('now'));

-- Ćevapi ingredients
INSERT OR IGNORE INTO "MenuItemIngredients" ("Id", "MenuItemId", "IngredientName", "QuantityInGrams", "IsMainIngredient", "DisplayOrder", "CreatedAt")
VALUES
    ('40000000-0000-0000-0000-000000000007', '20000000-0000-0000-0000-000000000002', 'Ground Beef', '150.00', 1, 1, datetime('now')),
    ('40000000-0000-0000-0000-000000000008', '20000000-0000-0000-0000-000000000002', 'Ground Lamb', '100.00', 1, 2, datetime('now')),
    ('40000000-0000-0000-0000-000000000009', '20000000-0000-0000-0000-000000000002', 'Somun Bread', '80.00', 1, 3, datetime('now')),
    ('40000000-0000-0000-0000-000000000010', '20000000-0000-0000-0000-000000000002', 'Onion', '30.00', 1, 4, datetime('now')),
    ('40000000-0000-0000-0000-000000000011', '20000000-0000-0000-0000-000000000002', 'Ajvar', '25.00', 0, 5, datetime('now'));

-- Šopska Salata ingredients
INSERT OR IGNORE INTO "MenuItemIngredients" ("Id", "MenuItemId", "IngredientName", "QuantityInGrams", "IsMainIngredient", "DisplayOrder", "CreatedAt")
VALUES
    ('40000000-0000-0000-0000-000000000012', '20000000-0000-0000-0000-000000000005', 'Tomatoes', '100.00', 1, 1, datetime('now')),
    ('40000000-0000-0000-0000-000000000013', '20000000-0000-0000-0000-000000000005', 'Cucumber', '80.00', 1, 2, datetime('now')),
    ('40000000-0000-0000-0000-000000000014', '20000000-0000-0000-0000-000000000005', 'Bell Peppers', '60.00', 1, 3, datetime('now')),
    ('40000000-0000-0000-0000-000000000015', '20000000-0000-0000-0000-000000000005', 'Sirene Cheese', '50.00', 1, 4, datetime('now')),
    ('40000000-0000-0000-0000-000000000016', '20000000-0000-0000-0000-000000000005', 'Olive Oil', '15.00', 0, 5, datetime('now')),
    ('40000000-0000-0000-0000-000000000017', '20000000-0000-0000-0000-000000000005', 'Red Onion', '20.00', 0, 6, datetime('now'));

-- Insert sample tables
INSERT OR IGNORE INTO "Tables" ("Id", "TableNumber", "Capacity", "Status")
VALUES
    ('30000000-0000-0000-0000-000000000001', 'T1', 4, 0),
    ('30000000-0000-0000-0000-000000000002', 'T2', 2, 0),
    ('30000000-0000-0000-0000-000000000003', 'T3', 6, 0),
    ('30000000-0000-0000-0000-000000000004', 'T4', 8, 0),
    ('30000000-0000-0000-0000-000000000005', 'T5', 4, 0),
    ('30000000-0000-0000-0000-000000000006', 'T6', 4, 0),
    ('30000000-0000-0000-0000-000000000007', 'T7', 2, 0),
    ('30000000-0000-0000-0000-000000000008', 'T8', 2, 0),
    ('30000000-0000-0000-0000-000000000009', 'T9', 6, 0),
    ('30000000-0000-0000-0000-000000000010', 'T10', 6, 0),
    ('30000000-0000-0000-0000-000000000011', 'T11', 4, 0),
    ('30000000-0000-0000-0000-000000000012', 'T12', 4, 0),
    ('30000000-0000-0000-0000-000000000013', 'T13', 2, 0),
    ('30000000-0000-0000-0000-000000000014', 'T14', 2, 0),
    ('30000000-0000-0000-0000-000000000015', 'T15', 8, 0),
    ('30000000-0000-0000-0000-000000000016', 'T16', 8, 0),
    ('30000000-0000-0000-0000-000000000017', 'T17', 4, 0),
    ('30000000-0000-0000-0000-000000000018', 'T18', 4, 0),
    ('30000000-0000-0000-0000-000000000019', 'T19', 6, 0),
    ('30000000-0000-0000-0000-000000000020', 'T20', 6, 0),
    ('30000000-0000-0000-0000-000000000021', 'T21', 2, 0),
    ('30000000-0000-0000-0000-000000000022', 'T22', 2, 0),
    ('30000000-0000-0000-0000-000000000023', 'T23', 4, 0),
    ('30000000-0000-0000-0000-000000000024', 'T24', 4, 0);

-- Display success message
SELECT 'SQLite database setup completed successfully!' as "Status";
SELECT 'All tables have been created: Restaurants, Users, Categories, MenuItems, Tables, Orders, OrderItems' as "TablesStatus";
SELECT 'Sample data has been inserted for testing!' as "SampleDataStatus";
SELECT 'The add-menu-item-submit-btn functionality is now ready!' as "MenuItemStatus";
