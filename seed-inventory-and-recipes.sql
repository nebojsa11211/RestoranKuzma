-- Seed script for Inventory Items and Recipes
-- This adds sample inventory items and creates recipes for existing menu items

-- Sample Inventory Items
INSERT INTO InventoryItems (Id, Name, Description, SKU, CurrentStock, Unit, MinimumStockLevel, UnitCost, IsActive, CreatedAt, UpdatedAt)
VALUES
-- Meat & Protein
('11111111-1111-1111-1111-111111111111', 'Chicken Breast', 'Fresh chicken breast', 'CHK-001', 50.0, 1, 10.0, 3.50, 1, datetime('now'), datetime('now')),
('11111111-1111-1111-1111-111111111112', 'Beef Steak', 'Prime beef steak', 'BEEF-001', 30.0, 1, 5.0, 8.00, 1, datetime('now'), datetime('now')),
('11111111-1111-1111-1111-111111111113', 'Salmon Fillet', 'Fresh salmon', 'FISH-001', 20.0, 1, 5.0, 12.00, 1, datetime('now'), datetime('now')),
('11111111-1111-1111-1111-111111111114', 'Ground Beef', 'Lean ground beef', 'BEEF-002', 40.0, 1, 10.0, 5.00, 1, datetime('now'), datetime('now')),

-- Vegetables
('22222222-2222-2222-2222-222222222221', 'Tomatoes', 'Fresh tomatoes', 'VEG-001', 15.0, 1, 5.0, 2.50, 1, datetime('now'), datetime('now')),
('22222222-2222-2222-2222-222222222222', 'Lettuce', 'Fresh lettuce', 'VEG-002', 10.0, 1, 3.0, 1.50, 1, datetime('now'), datetime('now')),
('22222222-2222-2222-2222-222222222223', 'Onions', 'Yellow onions', 'VEG-003', 20.0, 1, 5.0, 1.00, 1, datetime('now'), datetime('now')),
('22222222-2222-2222-2222-222222222224', 'Bell Peppers', 'Mixed bell peppers', 'VEG-004', 12.0, 1, 4.0, 2.00, 1, datetime('now'), datetime('now')),
('22222222-2222-2222-2222-222222222225', 'Mushrooms', 'Button mushrooms', 'VEG-005', 8.0, 1, 3.0, 3.50, 1, datetime('now'), datetime('now')),

-- Dairy & Cheese
('33333333-3333-3333-3333-333333333331', 'Mozzarella Cheese', 'Fresh mozzarella', 'DAIRY-001', 25.0, 1, 5.0, 4.50, 1, datetime('now'), datetime('now')),
('33333333-3333-3333-3333-333333333332', 'Parmesan Cheese', 'Aged parmesan', 'DAIRY-002', 15.0, 1, 3.0, 6.00, 1, datetime('now'), datetime('now')),
('33333333-3333-3333-3333-333333333333', 'Heavy Cream', 'Cooking cream', 'DAIRY-003', 10.0, 2, 3.0, 2.50, 1, datetime('now'), datetime('now')),
('33333333-3333-3333-3333-333333333334', 'Butter', 'Unsalted butter', 'DAIRY-004', 20.0, 1, 5.0, 3.00, 1, datetime('now'), datetime('now')),

-- Pasta & Grains
('44444444-4444-4444-4444-444444444441', 'Spaghetti Pasta', 'Dry spaghetti', 'PASTA-001', 50.0, 1, 10.0, 1.50, 1, datetime('now'), datetime('now')),
('44444444-4444-4444-4444-444444444442', 'Penne Pasta', 'Dry penne', 'PASTA-002', 50.0, 1, 10.0, 1.50, 1, datetime('now'), datetime('now')),
('44444444-4444-4444-4444-444444444443', 'Rice', 'Long grain rice', 'GRAIN-001', 100.0, 1, 20.0, 1.20, 1, datetime('now'), datetime('now')),
('44444444-4444-4444-4444-444444444444', 'Pizza Dough', 'Fresh pizza dough', 'DOUGH-001', 30.0, 1, 10.0, 2.00, 1, datetime('now'), datetime('now')),

-- Sauces & Condiments
('55555555-5555-5555-5555-555555555551', 'Tomato Sauce', 'Marinara sauce', 'SAUCE-001', 40.0, 2, 10.0, 1.80, 1, datetime('now'), datetime('now')),
('55555555-5555-5555-5555-555555555552', 'Olive Oil', 'Extra virgin olive oil', 'OIL-001', 20.0, 2, 5.0, 5.00, 1, datetime('now'), datetime('now')),
('55555555-5555-5555-5555-555555555553', 'Balsamic Vinegar', 'Aged balsamic', 'VIN-001', 10.0, 2, 3.0, 4.00, 1, datetime('now'), datetime('now')),
('55555555-5555-5555-5555-555555555554', 'Soy Sauce', 'Premium soy sauce', 'SAUCE-002', 15.0, 2, 5.0, 3.00, 1, datetime('now'), datetime('now')),

-- Beverages
('66666666-6666-6666-6666-666666666661', 'Coca Cola', 'Coke bottles 330ml', 'BEV-001', 100.0, 6, 20.0, 0.80, 1, datetime('now'), datetime('now')),
('66666666-6666-6666-6666-666666666662', 'Sprite', 'Sprite bottles 330ml', 'BEV-002', 100.0, 6, 20.0, 0.80, 1, datetime('now'), datetime('now')),
('66666666-6666-6666-6666-666666666663', 'Orange Juice', 'Fresh OJ 1L', 'BEV-003', 50.0, 6, 10.0, 2.00, 1, datetime('now'), datetime('now')),
('66666666-6666-6666-6666-666666666664', 'Coffee Beans', 'Premium arabica', 'BEV-004', 20.0, 1, 5.0, 15.00, 1, datetime('now'), datetime('now')),

-- Spices & Seasonings
('77777777-7777-7777-7777-777777777771', 'Salt', 'Table salt', 'SPICE-001', 50.0, 1, 10.0, 0.50, 1, datetime('now'), datetime('now')),
('77777777-7777-7777-7777-777777777772', 'Black Pepper', 'Ground black pepper', 'SPICE-002', 20.0, 1, 5.0, 2.00, 1, datetime('now'), datetime('now')),
('77777777-7777-7777-7777-777777777773', 'Garlic', 'Fresh garlic', 'SPICE-003', 15.0, 1, 5.0, 1.50, 1, datetime('now'), datetime('now')),
('77777777-7777-7777-7777-777777777774', 'Basil', 'Fresh basil', 'HERB-001', 5.0, 1, 2.0, 3.00, 1, datetime('now'), datetime('now')),
('77777777-7777-7777-7777-777777777775', 'Oregano', 'Dried oregano', 'HERB-002', 10.0, 1, 3.0, 2.50, 1, datetime('now'), datetime('now'));

-- Sample Recipes (linking menu items to inventory)
-- Note: Menu item IDs will need to be obtained from your actual database
-- These are example recipes - adjust based on your actual menu items

-- Example: Classic Margherita Pizza Recipe
-- Assuming menu item ID exists in your database
INSERT INTO Recipes (Id, MenuItemId, IsActive, CreatedAt, UpdatedAt)
SELECT
    lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6))),
    m.Id,
    1,
    datetime('now'),
    datetime('now')
FROM MenuItems m
WHERE m.Name = 'Margherita Pizza'
LIMIT 1;

-- Add ingredients for Margherita Pizza
INSERT INTO RecipeIngredients (Id, RecipeId, InventoryItemId, QuantityRequired, CreatedAt)
SELECT
    lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6))),
    r.Id,
    '44444444-4444-4444-4444-444444444444',
    0.3,
    datetime('now')
FROM Recipes r
JOIN MenuItems m ON r.MenuItemId = m.Id
WHERE m.Name = 'Margherita Pizza';

INSERT INTO RecipeIngredients (Id, RecipeId, InventoryItemId, QuantityRequired, CreatedAt)
SELECT
    lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6))),
    r.Id,
    '55555555-5555-5555-5555-555555555551',
    0.15,
    datetime('now')
FROM Recipes r
JOIN MenuItems m ON r.MenuItemId = m.Id
WHERE m.Name = 'Margherita Pizza';

INSERT INTO RecipeIngredients (Id, RecipeId, InventoryItemId, QuantityRequired, CreatedAt)
SELECT
    lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6))),
    r.Id,
    '33333333-3333-3333-3333-333333333331',
    0.2,
    datetime('now')
FROM Recipes r
JOIN MenuItems m ON r.MenuItemId = m.Id
WHERE m.Name = 'Margherita Pizza';

INSERT INTO RecipeIngredients (Id, RecipeId, InventoryItemId, QuantityRequired, CreatedAt)
SELECT
    lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6))),
    r.Id,
    '77777777-7777-7777-7777-777777777774',
    0.02,
    datetime('now')
FROM Recipes r
JOIN MenuItems m ON r.MenuItemId = m.Id
WHERE m.Name = 'Margherita Pizza';

-- Example: Grilled Chicken Recipe
INSERT INTO Recipes (Id, MenuItemId, IsActive, CreatedAt, UpdatedAt)
SELECT
    lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6))),
    m.Id,
    1,
    datetime('now'),
    datetime('now')
FROM MenuItems m
WHERE m.Name LIKE '%Chicken%'
LIMIT 1;

-- Add ingredients for Chicken dish
INSERT INTO RecipeIngredients (Id, RecipeId, InventoryItemId, QuantityRequired, CreatedAt)
SELECT
    lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6))),
    r.Id,
    '11111111-1111-1111-1111-111111111111',
    0.25,
    datetime('now')
FROM Recipes r
JOIN MenuItems m ON r.MenuItemId = m.Id
WHERE m.Name LIKE '%Chicken%'
LIMIT 1;

INSERT INTO RecipeIngredients (Id, RecipeId, InventoryItemId, QuantityRequired, CreatedAt)
SELECT
    lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6))),
    r.Id,
    '55555555-5555-5555-5555-555555555552',
    0.03,
    datetime('now')
FROM Recipes r
JOIN MenuItems m ON r.MenuItemId = m.Id
WHERE m.Name LIKE '%Chicken%'
LIMIT 1;

-- Verification queries
SELECT '=== Inventory Items Added ===' AS Status;
SELECT COUNT(*) AS TotalInventoryItems FROM InventoryItems;

SELECT '=== Recipes Created ===' AS Status;
SELECT COUNT(*) AS TotalRecipes FROM Recipes;

SELECT '=== Recipe Ingredients ===' AS Status;
SELECT COUNT(*) AS TotalRecipeIngredients FROM RecipeIngredients;

SELECT '=== Sample Recipe Details ===' AS Status;
SELECT
    m.Name AS MenuItem,
    i.Name AS Ingredient,
    ri.QuantityRequired,
    i.Unit
FROM Recipes r
JOIN MenuItems m ON r.MenuItemId = m.Id
JOIN RecipeIngredients ri ON ri.RecipeId = r.Id
JOIN InventoryItems i ON i.Id = ri.InventoryItemId
ORDER BY m.Name, i.Name;
