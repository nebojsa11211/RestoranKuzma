-- Database Migration: Multi-Restaurant to Single-Restaurant Architecture (PostgreSQL)
-- Date: 2025-10-06
-- Description: Removes RestaurantId foreign keys and updates schema for single restaurant

-- ==================================================
-- STEP 1: DROP FOREIGN KEY CONSTRAINTS
-- ==================================================
\echo 'Dropping foreign key constraints...'

-- Note: Check actual constraint names in your database first
DO $$
BEGIN
    -- Drop FK from Users table (if exists)
    IF EXISTS (SELECT 1 FROM information_schema.table_constraints
               WHERE constraint_name = 'FK_Users_Restaurants_RestaurantId'
               AND table_name = 'Users') THEN
        ALTER TABLE "Users" DROP CONSTRAINT "FK_Users_Restaurants_RestaurantId";
    END IF;

    -- Drop FK from Categories table (if exists)
    IF EXISTS (SELECT 1 FROM information_schema.table_constraints
               WHERE constraint_name = 'FK_Categories_Restaurants_RestaurantId'
               AND table_name = 'Categories') THEN
        ALTER TABLE "Categories" DROP CONSTRAINT "FK_Categories_Restaurants_RestaurantId";
    END IF;

    -- Drop FK from MenuItems table (if exists)
    IF EXISTS (SELECT 1 FROM information_schema.table_constraints
               WHERE constraint_name = 'FK_MenuItems_Restaurants_RestaurantId'
               AND table_name = 'MenuItems') THEN
        ALTER TABLE "MenuItems" DROP CONSTRAINT "FK_MenuItems_Restaurants_RestaurantId";
    END IF;

    -- Drop FK from Orders table (if exists)
    IF EXISTS (SELECT 1 FROM information_schema.table_constraints
               WHERE constraint_name = 'FK_Orders_Restaurants_RestaurantId'
               AND table_name = 'Orders') THEN
        ALTER TABLE "Orders" DROP CONSTRAINT "FK_Orders_Restaurants_RestaurantId";
    END IF;

    -- Drop FK from Tables table (if exists)
    IF EXISTS (SELECT 1 FROM information_schema.table_constraints
               WHERE constraint_name = 'FK_Tables_Restaurants_RestaurantId'
               AND table_name = 'Tables') THEN
        ALTER TABLE "Tables" DROP CONSTRAINT "FK_Tables_Restaurants_RestaurantId";
    END IF;
END $$;

\echo 'Foreign key constraints dropped (if they existed).'

-- ==================================================
-- STEP 2: DROP OLD COMPOSITE INDEXES
-- ==================================================
\echo 'Dropping old composite indexes...'

DROP INDEX IF EXISTS "IX_Users_RestaurantId_Role";
DROP INDEX IF EXISTS "IX_Categories_RestaurantId_DisplayOrder";
DROP INDEX IF EXISTS "IX_MenuItems_RestaurantId_IsAvailable";
DROP INDEX IF EXISTS "IX_Orders_RestaurantId_Status_CreatedAt";
DROP INDEX IF EXISTS "IX_Tables_RestaurantId_TableNumber";

\echo 'Old indexes dropped.'

-- ==================================================
-- STEP 3: DROP RestaurantId COLUMNS
-- ==================================================
\echo 'Dropping RestaurantId columns...'

ALTER TABLE "Users" DROP COLUMN IF EXISTS "RestaurantId";
ALTER TABLE "Categories" DROP COLUMN IF EXISTS "RestaurantId";
ALTER TABLE "MenuItems" DROP COLUMN IF EXISTS "RestaurantId";
ALTER TABLE "Orders" DROP COLUMN IF EXISTS "RestaurantId";
ALTER TABLE "Tables" DROP COLUMN IF EXISTS "RestaurantId";

\echo 'RestaurantId columns dropped.'

-- ==================================================
-- STEP 4: UPDATE RESTAURANT TABLE
-- ==================================================
\echo 'Updating Restaurant table structure...'

-- Add new columns to Restaurant table (if they don't exist)
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                   WHERE table_name = 'Restaurants' AND column_name = 'Timezone') THEN
        ALTER TABLE "Restaurants" ADD COLUMN "Timezone" VARCHAR(100) NOT NULL DEFAULT 'UTC';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                   WHERE table_name = 'Restaurants' AND column_name = 'Currency') THEN
        ALTER TABLE "Restaurants" ADD COLUMN "Currency" VARCHAR(3) NOT NULL DEFAULT 'USD';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM information_schema.columns
                   WHERE table_name = 'Restaurants' AND column_name = 'SettingsJson') THEN
        ALTER TABLE "Restaurants" ADD COLUMN "SettingsJson" TEXT NOT NULL DEFAULT '{}';
    END IF;
END $$;

-- Drop old columns (if they exist)
ALTER TABLE "Restaurants" DROP COLUMN IF EXISTS "Phone";
ALTER TABLE "Restaurants" DROP COLUMN IF EXISTS "IsActive";

\echo 'Restaurant table updated.'

-- ==================================================
-- STEP 5: CREATE NEW SIMPLIFIED INDEXES
-- ==================================================
\echo 'Creating new simplified indexes...'

-- Create unique index on Email for Users
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users"("Email");

-- Create index on DisplayOrder for Categories
CREATE INDEX IF NOT EXISTS "IX_Categories_DisplayOrder" ON "Categories"("DisplayOrder");

-- Create composite index on CategoryId and IsAvailable for MenuItems
CREATE INDEX IF NOT EXISTS "IX_MenuItems_CategoryId_IsAvailable" ON "MenuItems"("CategoryId", "IsAvailable");

-- Create composite index on Status and CreatedAt for Orders
CREATE INDEX IF NOT EXISTS "IX_Orders_Status_CreatedAt" ON "Orders"("Status", "CreatedAt");

-- Create unique index on TableNumber for Tables
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Tables_TableNumber" ON "Tables"("TableNumber");

\echo 'New indexes created.'

-- ==================================================
-- STEP 6: DATA CLEANUP
-- ==================================================
\echo 'Checking restaurant data...'

DO $$
DECLARE
    restaurant_count INT;
    main_restaurant_id UUID;
BEGIN
    SELECT COUNT(*) INTO restaurant_count FROM "Restaurants";

    IF restaurant_count > 1 THEN
        RAISE NOTICE 'WARNING: Multiple restaurants found (%). Keeping the first one created.', restaurant_count;

        SELECT "Id" INTO main_restaurant_id
        FROM "Restaurants"
        ORDER BY "CreatedAt"
        LIMIT 1;

        RAISE NOTICE 'Deleting additional restaurants...';
        DELETE FROM "Restaurants" WHERE "Id" != main_restaurant_id;
    END IF;

    -- Update restaurant name to "Restoran Kuzma" if not already
    UPDATE "Restaurants"
    SET "Name" = 'Restoran Kuzma'
    WHERE "Name" != 'Restoran Kuzma';
END $$;

\echo 'Data cleanup completed.'

-- ==================================================
-- STEP 7: VERIFICATION
-- ==================================================
\echo 'Verifying migration...'

-- Check restaurant count (should be 1 or 0)
SELECT 'Restaurant Count' as "Check", COUNT(*) as "Result" FROM "Restaurants";

-- Check for any remaining RestaurantId columns (should be none)
SELECT
    table_name AS "TableName",
    column_name AS "ColumnName"
FROM information_schema.columns
WHERE column_name = 'RestaurantId'
  AND table_schema = 'public'
  AND table_name NOT IN ('Restaurants');

\echo '============================================'
\echo 'Migration to single-restaurant architecture completed!'
\echo '============================================'
\echo 'Next steps:'
\echo '1. Verify application builds successfully'
\echo '2. Test database connections'
\echo '3. Run integration tests'
\echo '============================================'
