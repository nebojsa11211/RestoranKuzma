-- Create a chef user for testing
INSERT INTO "Users" ("Id", "Email", "FirstName", "LastName", "PasswordHash", "Role", "IsActive", "CreatedAt") 
VALUES (gen_random_uuid(), 'chef@restaurant.com', 'Chef', 'Master', 'temp_hash', 2, true, NOW());

-- Check if the user was created
SELECT "Email", "FirstName", "LastName", "Role" FROM "Users" WHERE "Email" = 'chef@restaurant.com';
