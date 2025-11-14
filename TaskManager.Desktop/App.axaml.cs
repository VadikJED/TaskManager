using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TaskManager.Desktop.ViewModels;
using TaskManager.Desktop.Views;

namespace TaskManager.Desktop;

public partial class App : Application
{
  private IServiceProvider? _services;

  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    // Получаем сервис-провайдер
    _services = Program.ConfigureServices();

    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      // Убираем валидацию, которая может мешать биндингам
      DisableAvaloniaDataAnnotationValidation();

      // Создаем главное окно
      var mainWindow = new MainWindow
      {
        DataContext = _services.GetRequiredService<MainWindowViewModel>()
      };

      desktop.MainWindow = mainWindow;
    }

    base.OnFrameworkInitializationCompleted();
  }

  private static void DisableAvaloniaDataAnnotationValidation()
  {
    // Отключаем плагин валидации данных Avalonia, который может конфликтовать с CommunityToolkit.Mvvm
    var dataValidationPluginsToRemove = BindingPlugins.DataValidators.ToList();
    foreach (var plugin in dataValidationPluginsToRemove)
    {
      BindingPlugins.DataValidators.Remove(plugin);
    }
  }
}