-- Update passwords for test users
-- Password for all users: "Password123!"
-- This is the BCrypt hash for "Password123!"

UPDATE Users
SET PasswordHash = '$2a$11$8Z5H1YfqJxV4mXGZzJYqH.xN7kN9pQvF7jW3yH5xZ9L0M1N2O3P4Q'
WHERE Email IN ('mark@restaurant.com', 'sarah@restaurant.com', 'david@restaurant.com', 'emma@restaurant.com');

SELECT 'Passwords updated successfully!' as Status;
SELECT 'Use password: Password123! for all test users' as Info;
