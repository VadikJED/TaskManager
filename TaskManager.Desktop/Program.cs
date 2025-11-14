using Avalonia;
using System;
using TaskManager.Desktop.Data;
using TaskManager.Desktop.Repositories;
using TaskManager.Desktop.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Desktop.ViewModels;
using TaskManager.Desktop.Views;

namespace TaskManager.Desktop;

class Program
{
  [STAThread]
  public static void Main(string[] args)
  {
    try
    {
      // Создаем сервисы
      var services = ConfigureServices();

      // Инициализируем базу данных
      DatabaseService.InitializeDatabase(services);

      // Запускаем Avalonia
      BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Application startup failed: {ex}");
      throw;
    }
  }

  public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
      .UsePlatformDetect()
      .WithInterFont()
      .LogToTrace();

  public static ServiceProvider ConfigureServices()
  {
    var configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json")
      .Build();

    var services = new ServiceCollection();

    // Регистрируем DbContext
    services.AddDbContext<ApplicationDbContext>(options =>
      options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

    // Регистрируем репозиторий
    services.AddScoped<ITaskRepository, TaskRepository>();

    // Регистрируем ViewModels
    services.AddTransient<MainWindowViewModel>();

    return services.BuildServiceProvider();
  }
}