using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Linq;
using TaskManager.Desktop.Data;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Services;

public class DatabaseService
{
  public static void InitializeDatabase(IServiceProvider serviceProvider)
  {
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseService>>();

    try
    {
      logger.LogInformation("🔄 Starting database initialization...");

      // 1. Проверяем/создаем базу данных
      EnsureDatabaseCreated(logger);

      // 2. Применяем миграции
      try
      {
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("✅ Database migrations applied successfully");
      }
      catch (Exception migrateEx)
      {
        logger.LogWarning(migrateEx, "Migrations failed, creating table manually...");
        CreateTablesManually(context, logger);
      }

      // 3. Проверяем существование таблицы Tasks
      if (!TableExists(context, "Tasks"))
      {
        logger.LogWarning("Tasks table still doesn't exist, creating...");
        CreateTablesManually(context, logger);
      }

      // 4. Добавляем тестовые данные
      if (!context.Tasks.Any())
      {
        logger.LogInformation("Adding sample data...");
        AddSampleData(context, logger);
      }

      logger.LogInformation($"✅ Database initialization completed. Tasks count: {context.Tasks.Count()}");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "❌ Database initialization failed");
    }
  }

  private static void EnsureDatabaseCreated(ILogger logger)
  {
    try
    {
      var masterConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=your_password;";

      using var connection = new NpgsqlConnection(masterConnectionString);
      connection.Open();

      using var checkCmd = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = 'taskmanager'", connection);
      var exists = checkCmd.ExecuteScalar() != null;

      if (!exists)
      {
        logger.LogInformation("Creating database 'taskmanager'...");
        using var createCmd = new NpgsqlCommand("CREATE DATABASE taskmanager", connection);
        createCmd.ExecuteNonQuery();
        logger.LogInformation("✅ Database 'taskmanager' created");
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to ensure database exists");
    }
  }

  private static void CreateTablesManually(ApplicationDbContext context, ILogger logger)
  {
    try
    {
      // Создаем таблицу Tasks
      context.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS ""Tasks"" (
                    ""Id"" UUID PRIMARY KEY,
                    ""Title"" VARCHAR(100) NOT NULL,
                    ""IsCompleted"" BOOLEAN NOT NULL,
                    ""CreatedAt"" TIMESTAMP WITHOUT TIME ZONE NOT NULL
                )");

      logger.LogInformation("✅ Tasks table created manually");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "❌ Failed to create tables manually");
    }
  }

  private static bool TableExists(ApplicationDbContext context, string tableName)
  {
    try
    {
      var result = context.Database.SqlQueryRaw<int>(
          $"SELECT COUNT(*) FROM information_schema.tables WHERE table_name = '{tableName.ToLower()}'");
      return result.First() > 0;
    }
    catch
    {
      return false;
    }
  }

  private static void AddSampleData(ApplicationDbContext context, ILogger logger)
  {
    try
    {
      var sampleTasks = new[]
      {
                new TaskItem {
                    Title = "Learn Avalonia UI",
                    IsCompleted = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new TaskItem {
                    Title = "Setup Entity Framework Core",
                    IsCompleted = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new TaskItem {
                    Title = "Create Task Manager Application",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-5)
                }
            };

      context.Tasks.AddRange(sampleTasks);
      context.SaveChanges();
      logger.LogInformation("✅ Sample data added successfully");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "❌ Failed to add sample data");
    }
  }
}