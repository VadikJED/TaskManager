using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Desktop.Data;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Services;

public class DatabaseService
{
  public static void InitializeDatabase(IServiceProvider serviceProvider)
  {
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
      Console.WriteLine("🔄 Checking database...");

      var canConnect = context.Database.CanConnect();
      Console.WriteLine($"✅ Database connection: {canConnect}");

      Console.WriteLine("🔄 Applying migrations...");
      context.Database.Migrate();
      Console.WriteLine("✅ Migrations applied");

      // Add sample data if table is empty
      if (!context.Tasks.Any())
      {
        Console.WriteLine("🔄 Adding sample data...");
        context.Tasks.AddRange(
            new TaskItem
            {
              Title = "Learn Avalonia UI",
              IsCompleted = true,
              CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new TaskItem
            {
              Title = "Setup Entity Framework Core",
              IsCompleted = true,
              CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new TaskItem
            {
              Title = "Create Task Manager",
              IsCompleted = false,
              CreatedAt = DateTime.UtcNow.AddHours(-5)
            },
            new TaskItem
            {
              Title = "Add delete functionality",
              IsCompleted = false,
              CreatedAt = DateTime.UtcNow.AddHours(-2)
            }
        );
        context.SaveChanges();
        Console.WriteLine("✅ Sample data added");
      }

      Console.WriteLine($"✅ Database ready. Tasks in database: {context.Tasks.Count()}");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Database initialization error: {ex.Message}");

      try
      {
        Console.WriteLine("🔄 Trying to create database via EnsureCreated...");
        context.Database.EnsureCreated();
        Console.WriteLine("✅ Database created via EnsureCreated");
      }
      catch (Exception createEx)
      {
        Console.WriteLine($"❌ Failed to create database: {createEx.Message}");
      }
    }
  }
}