using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using TaskManager.Desktop.Data;
using TaskManager.Desktop.Repositories;
using TaskManager.Desktop.Services;
using TaskManager.Desktop.ViewModels;
using TaskManager.Desktop.Views;

namespace TaskManager.Desktop;

public partial class App : Application
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    // Remove DataAnnotations validation
    DisableAvaloniaDataAnnotationValidation();

    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      // Получаем сервис-провайдер через публичное свойство
      var services = Program.Services;
      if (services == null)
      {
        throw new InvalidOperationException("ServiceProvider is not initialized");
      }

      // Log application start
      var logger = services.GetRequiredService<ILogger<App>>();
      logger.LogInformation("?? Task Manager application starting...");

      desktop.MainWindow = new MainWindow
      {
        DataContext = services.GetRequiredService<MainWindowViewModel>()
      };

      logger.LogInformation("? Main window initialized");
    }

    base.OnFrameworkInitializationCompleted();
  }

  private static void DisableAvaloniaDataAnnotationValidation()
  {
    var dataValidationPluginsToRemove = BindingPlugins.DataValidators.ToList();
    foreach (var plugin in dataValidationPluginsToRemove)
    {
      BindingPlugins.DataValidators.Remove(plugin);
    }
  }
}