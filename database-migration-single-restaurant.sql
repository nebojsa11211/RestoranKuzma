-- Database Migration: Multi-Restaurant to Single-Restaurant Architecture
-- Date: 2025-10-06
-- Description: Removes RestaurantId foreign keys and updates schema for single restaurant

-- ==================================================
-- STEP 1: BACKUP VERIFICATION
-- ==================================================
-- Before running this script, ensure you have a database backup!

PRINT 'Starting migration to single-restaurant architecture...';
GO

-- ==================================================
-- STEP 2: DROP FOREIGN KEY CONSTRAINTS
-- ==================================================
PRINT 'Dropping foreign key constraints...';

-- Drop FK from Users table
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Users_Restaurants_RestaurantId')
    ALTER TABLE [Users] DROP CONSTRAINT [FK_Users_Restaurants_RestaurantId];

-- Drop FK from Categories table
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Categories_Restaurants_RestaurantId')
    ALTER TABLE [Categories] DROP CONSTRAINT [FK_Categories_Restaurants_RestaurantId];

-- Drop FK from MenuItems table
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_MenuItems_Restaurants_RestaurantId')
    ALTER TABLE [MenuItems] DROP CONSTRAINT [FK_MenuItems_Restaurants_RestaurantId];

-- Drop FK from Orders table
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_Restaurants_RestaurantId')
    ALTER TABLE [Orders] DROP CONSTRAINT [FK_Orders_Restaurants_RestaurantId];

-- Drop FK from Tables table
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tables_Restaurants_RestaurantId')
    ALTER TABLE [Tables] DROP CONSTRAINT [FK_Tables_Restaurants_RestaurantId];

PRINT 'Foreign key constraints dropped successfully.';
GO

-- ==================================================
-- STEP 3: DROP OLD COMPOSITE INDEXES
-- ==================================================
PRINT 'Dropping old composite indexes...';

-- Drop indexes with RestaurantId
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_RestaurantId_Role')
    DROP INDEX [IX_Users_RestaurantId_Role] ON [Users];

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Categories_RestaurantId_DisplayOrder')
    DROP INDEX [IX_Categories_RestaurantId_DisplayOrder] ON [Categories];

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MenuItems_RestaurantId_IsAvailable')
    DROP INDEX [IX_MenuItems_RestaurantId_IsAvailable] ON [MenuItems];

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_RestaurantId_Status_CreatedAt')
    DROP INDEX [IX_Orders_RestaurantId_Status_CreatedAt] ON [Orders];

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tables_RestaurantId_TableNumber')
    DROP INDEX [IX_Tables_RestaurantId_TableNumber] ON [Tables];

PRINT 'Old indexes dropped successfully.';
GO

-- ==================================================
-- STEP 4: DROP RestaurantId COLUMNS
-- ==================================================
PRINT 'Dropping RestaurantId columns...';

-- Drop RestaurantId from Users (if exists)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Users') AND name = 'RestaurantId')
    ALTER TABLE [Users] DROP COLUMN [RestaurantId];

-- Drop RestaurantId from Categories
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Categories') AND name = 'RestaurantId')
    ALTER TABLE [Categories] DROP COLUMN [RestaurantId];

-- Drop RestaurantId from MenuItems
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('MenuItems') AND name = 'RestaurantId')
    ALTER TABLE [MenuItems] DROP COLUMN [RestaurantId];

-- Drop RestaurantId from Orders
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Orders') AND name = 'RestaurantId')
    ALTER TABLE [Orders] DROP COLUMN [RestaurantId];

-- Drop RestaurantId from Tables
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Tables') AND name = 'RestaurantId')
    ALTER TABLE [Tables] DROP COLUMN [RestaurantId];

PRINT 'RestaurantId columns dropped successfully.';
GO

-- ==================================================
-- STEP 5: UPDATE RESTAURANT TABLE
-- ==================================================
PRINT 'Updating Restaurant table structure...';

-- Add new columns to Restaurant table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Restaurants') AND name = 'Timezone')
    ALTER TABLE [Restaurants] ADD [Timezone] NVARCHAR(100) NOT NULL DEFAULT 'UTC';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Restaurants') AND name = 'Currency')
    ALTER TABLE [Restaurants] ADD [Currency] NVARCHAR(3) NOT NULL DEFAULT 'USD';

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Restaurants') AND name = 'SettingsJson')
    ALTER TABLE [Restaurants] ADD [SettingsJson] NVARCHAR(MAX) NOT NULL DEFAULT '{}';

-- Drop old columns if they exist
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Restaurants') AND name = 'Phone')
    ALTER TABLE [Restaurants] DROP COLUMN [Phone];

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Restaurants') AND name = 'IsActive')
    ALTER TABLE [Restaurants] DROP COLUMN [IsActive];

PRINT 'Restaurant table updated successfully.';
GO

-- ==================================================
-- STEP 6: CREATE NEW SIMPLIFIED INDEXES
-- ==================================================
PRINT 'Creating new simplified indexes...';

-- Create unique index on Email for Users
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users]([Email]);

-- Create index on DisplayOrder for Categories
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Categories_DisplayOrder' AND object_id = OBJECT_ID('Categories'))
    CREATE INDEX [IX_Categories_DisplayOrder] ON [Categories]([DisplayOrder]);

-- Create composite index on CategoryId and IsAvailable for MenuItems
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MenuItems_CategoryId_IsAvailable' AND object_id = OBJECT_ID('MenuItems'))
    CREATE INDEX [IX_MenuItems_CategoryId_IsAvailable] ON [MenuItems]([CategoryId], [IsAvailable]);

-- Create composite index on Status and CreatedAt for Orders
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_Status_CreatedAt' AND object_id = OBJECT_ID('Orders'))
    CREATE INDEX [IX_Orders_Status_CreatedAt] ON [Orders]([Status], [CreatedAt]);

-- Create unique index on TableNumber for Tables
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tables_TableNumber' AND object_id = OBJECT_ID('Tables'))
    CREATE UNIQUE INDEX [IX_Tables_TableNumber] ON [Tables]([TableNumber]);

PRINT 'New indexes created successfully.';
GO

-- ==================================================
-- STEP 7: DATA CLEANUP (IF NEEDED)
-- ==================================================
PRINT 'Checking restaurant data...';

-- Ensure only one restaurant exists
DECLARE @RestaurantCount INT;
SELECT @RestaurantCount = COUNT(*) FROM [Restaurants];

IF @RestaurantCount > 1
BEGIN
    PRINT 'WARNING: Multiple restaurants found. Keeping the first one created.';
    DECLARE @MainRestaurantId UNIQUEIDENTIFIER;
    SELECT TOP 1 @MainRestaurantId = Id FROM [Restaurants] ORDER BY CreatedAt;

    PRINT 'Deleting additional restaurants...';
    DELETE FROM [Restaurants] WHERE Id != @MainRestaurantId;
END

-- Update restaurant name to "Restoran Kuzma" if it's not already
UPDATE [Restaurants] SET [Name] = 'Restoran Kuzma' WHERE [Name] != 'Restoran Kuzma';

PRINT 'Data cleanup completed.';
GO

-- ==================================================
-- STEP 8: VERIFICATION
-- ==================================================
PRINT 'Verifying migration...';

-- Check restaurant count
SELECT 'Restaurant Count' as [Check], COUNT(*) as [Result] FROM [Restaurants];

-- Check for any remaining RestaurantId columns (should be none)
SELECT
    t.name AS TableName,
    c.name AS ColumnName
FROM sys.columns c
INNER JOIN sys.tables t ON c.object_id = t.object_id
WHERE c.name = 'RestaurantId'
  AND t.name NOT IN ('Restaurants');

PRINT '============================================';
PRINT 'Migration to single-restaurant architecture completed successfully!';
PRINT '============================================';
PRINT 'Next steps:';
PRINT '1. Verify application builds successfully';
PRINT '2. Test database connections';
PRINT '3. Run integration tests';
PRINT '============================================';
GO
