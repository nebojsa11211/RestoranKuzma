-- SQLite Database Initialization Script - Simplified
-- This script creates the Users table for staff members

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

-- Insert sample staff members for testing
INSERT OR IGNORE INTO "Users" ("Id", "Email", "FirstName", "LastName", "PasswordHash", "Role", "IsActive", "Phone", "CreatedAt")
VALUES 
    ('00000000-0000-0000-0000-000000000001', 'mark@restaurant.com', 'Mark', 'Johnson', 'temp_hash', 0, 1, '+381 11 123 4567', datetime('now')),
    ('00000000-0000-0000-0000-000000000002', 'sarah@restaurant.com', 'Sarah', 'Williams', 'temp_hash', 1, 1, '+381 11 123 4568', datetime('now')),
    ('00000000-0000-0000-0000-000000000003', 'david@restaurant.com', 'David', 'Brown', 'temp_hash', 0, 1, '+381 11 123 4569', datetime('now')),
    ('00000000-0000-0000-0000-000000000004', 'emma@restaurant.com', 'Emma', 'Wilson', 'temp_hash', 2, 1, '+381 11 123 4570', datetime('now'));
