-- Database Setup Script for RestaurantSuite
-- This script creates the necessary database and tables for staff persistence

-- Connect to PostgreSQL and run this script
-- psql -U postgres -d postgres -f setup-database.sql

-- Create database if it doesn't exist
SELECT 'CREATE DATABASE restorankuzma'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'restorankuzma')\gexec

-- Connect to the database
\c restorankuzma;

-- Create extensions if they don't exist
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create Users table (for staff members)
CREATE TABLE IF NOT EXISTS public."Users" (
    "Id" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "Email" character varying(256) NOT NULL,
    "FirstName" character varying(100) NOT NULL,
    "LastName" character varying(100) NOT NULL,
    "Phone" character varying(20),
    "PasswordHash" text NOT NULL,
    "RefreshToken" character varying(500),
    "RefreshTokenExpiresAt" timestamp with time zone,
    "Role" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id"),
    CONSTRAINT "IX_Users_Email" UNIQUE ("Email")
);

-- Create indexes
CREATE INDEX IF NOT EXISTS "IX_Users_RefreshToken" ON public."Users" ("RefreshToken");

-- Insert sample staff members for testing
INSERT INTO public."Users" ("Email", "FirstName", "LastName", "PasswordHash", "Role", "IsActive", "Phone", "CreatedAt")
VALUES 
    ('mark@restaurant.com', 'Mark', 'Johnson', 'temp_hash', 0, true, '+381 11 123 4567', NOW()),
    ('sarah@restaurant.com', 'Sarah', 'Williams', 'temp_hash', 1, true, '+381 11 123 4568', NOW()),
    ('david@restaurant.com', 'David', 'Brown', 'temp_hash', 0, true, '+381 11 123 4569', NOW()),
    ('emma@restaurant.com', 'Emma', 'Wilson', 'temp_hash', 2, true, '+381 11 123 4570', NOW())
ON CONFLICT ("Email") DO NOTHING;

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
FROM public."Users"
ORDER BY "CreatedAt" DESC;

-- Grant permissions (adjust username as needed)
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO postgres;

-- Create other required tables for the application
CREATE TABLE IF NOT EXISTS public."Restaurants" (
    "Id" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "Name" character varying(200) NOT NULL,
    "Address" character varying(500) NOT NULL,
    "Timezone" character varying(100) NOT NULL,
    "Currency" character varying(3) NOT NULL,
    "SettingsJson" text,
    CONSTRAINT "PK_Restaurants" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public."Categories" (
    "Id" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "Name" character varying(100) NOT NULL,
    "DisplayOrder" integer NOT NULL,
    CONSTRAINT "PK_Categories" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS public."MenuItems" (
    "Id" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "Name" character varying(150) NOT NULL,
    "Description" character varying(500),
    "Price" numeric(18,2) NOT NULL,
    "ImageUrl" character varying(500),
    "IsAvailable" boolean NOT NULL,
    "CategoryId" uuid NOT NULL,
    CONSTRAINT "PK_MenuItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MenuItems_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES public."Categories" ("Id") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS public."Tables" (
    "Id" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "TableNumber" character varying(50) NOT NULL,
    "Capacity" integer NOT NULL,
    "Status" integer NOT NULL,
    CONSTRAINT "PK_Tables" PRIMARY KEY ("Id"),
    CONSTRAINT "IX_Tables_TableNumber" UNIQUE ("TableNumber")
);

CREATE TABLE IF NOT EXISTS public."Orders" (
    "Id" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "TableId" uuid NOT NULL,
    "WaiterId" uuid NOT NULL,
    "GuestId" uuid,
    "Status" integer NOT NULL,
    "TotalAmount" numeric(18,2) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
    "UpdatedAt" timestamp with time zone,
    CONSTRAINT "PK_Orders" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Orders_Tables_TableId" FOREIGN KEY ("TableId") REFERENCES public."Tables" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Orders_Users_WaiterId" FOREIGN KEY ("WaiterId") REFERENCES public."Users" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Orders_Users_GuestId" FOREIGN KEY ("GuestId") REFERENCES public."Users" ("Id") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS public."OrderItems" (
    "Id" uuid NOT NULL DEFAULT uuid_generate_v4(),
    "OrderId" uuid NOT NULL,
    "MenuItemId" uuid NOT NULL,
    "Quantity" integer NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "Subtotal" numeric(18,2) NOT NULL,
    "SpecialInstructions" character varying(500),
    CONSTRAINT "PK_OrderItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OrderItems_MenuItems_MenuItemId" FOREIGN KEY ("MenuItemId") REFERENCES public."MenuItems" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES public."Orders" ("Id") ON DELETE CASCADE
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS "IX_MenuItems_CategoryId_IsAvailable" ON public."MenuItems" ("CategoryId", "IsAvailable");
CREATE INDEX IF NOT EXISTS "IX_Orders_Status_CreatedAt" ON public."Orders" ("Status", "CreatedAt");

-- Display success message
SELECT 'Database setup completed successfully!' as "Status";
SELECT 'Staff members table is ready for use!' as "StaffTableStatus";
SELECT 'Sample staff members have been inserted!' as "SampleDataStatus";
