#!/usr/bin/env dotnet-script
using System;
using System.Security.Cryptography;
using System.Text;

// Generate SHA256 hash for a password (matching JwtAuthenticationService implementation)
string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(hashedBytes);
}

// Generate hashes for test password
string testPassword = "password123";
string hash = HashPassword(testPassword);

Console.WriteLine($"Password: {testPassword}");
Console.WriteLine($"SHA256 Hash: {hash}");
Console.WriteLine();
Console.WriteLine("Test Users with this password:");
Console.WriteLine("- sarah@restaurant.com (Waiter)");
Console.WriteLine("- mark@restaurant.com (Admin)");
Console.WriteLine("- david@restaurant.com (Admin)");
Console.WriteLine("- emma@restaurant.com (Chef)");
