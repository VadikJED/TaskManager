using Avalonia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using TaskManager.Desktop.Data;
using TaskManager.Desktop.Repositories;
using TaskManager.Desktop.Services;
using TaskManager.Desktop.ViewModels;
using TaskManager.Desktop.Views;

namespace TaskManager.Desktop;

class Program
{
  private static ServiceProvider? _serviceProvider;

  [STAThread]
  public static void Main(string[] args)
  {
    try
    {
      // Создаем и настраиваем хост приложения
      _serviceProvider = ConfigureServices();

      // Инициализируем базу данных
      DatabaseService.InitializeDatabase(_serviceProvider);

      // Запускаем Avalonia
      BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"❌ Application startup failed: {ex}");
      throw;
    }
  }

  public static AppBuilder BuildAvaloniaApp()
      => AppBuilder.Configure<App>()
          .UsePlatformDetect()
          .WithInterFont()
          .LogToTrace();

  private static ServiceProvider ConfigureServices()
  {
    // Build configuration
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    var services = new ServiceCollection();

    // Add Logging
    services.AddLogging(builder =>
    {
      builder.AddConfiguration(configuration.GetSection("Logging"));
      builder.AddConsole();
      builder.AddDebug();
      builder.SetMinimumLevel(LogLevel.Information);
    });

    // Register DbContext
    services.AddDbContext<ApplicationDbContext>(options =>
    {
      options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

      // Enable sensitive data logging only in development
#if DEBUG
      options.EnableSensitiveDataLogging();
#endif
    });

    // Register repositories
    services.AddScoped<ITaskRepository, TaskRepository>();

    // Register ViewModels
    services.AddTransient<MainWindowViewModel>();

    return services.BuildServiceProvider();
  }

  // Публичное свойство для доступа к ServiceProvider
  public static ServiceProvider? Services => _serviceProvider;
}