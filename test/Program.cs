using System;
using Microsoft.AspNetCore.Identity;

// Test cod to show how params object?[] works with a custom function
class Program
{
    static void Main()
    {
        PrintValues("Single value", 123);
        PrintValues("Multiple values", "Alice", DateTime.Now, true);
        PrintValues("No extra values");

        // Wait for user input
        Console.Write("Enter something: ");
        string userInput = Console.ReadLine(); // Reads input from the console
        Console.WriteLine($"You entered: {userInput}"); // Prints the user input

        var hasher = new PasswordHasher<IdentityUser>();
        string hashedPassword = hasher.HashPassword(null, "P@ssword1");
        Console.WriteLine($"Hashed Password: {hashedPassword}");

        Console.Write("Enter something: ");
    }


    static void PrintValues(string message, params object?[] values)
    {
        Console.WriteLine(message);
        foreach (var value in values)
        {
            Console.WriteLine($"  - {value}");
        }
    }
}
