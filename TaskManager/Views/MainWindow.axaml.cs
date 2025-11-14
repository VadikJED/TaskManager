using Avalonia.Controls;
using Avalonia.Input;
using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using TaskManager.Models;
using TaskManager.ViewModels;

namespace TaskManager.Views;

public partial class MainWindow : Window
{
  private MainViewModel? _viewModel;

  public MainWindow()
  {
    InitializeComponent();
    _viewModel = new MainViewModel();
    DataContext = _viewModel;
  }

  private void OnNewTaskKeyDown(object? sender, KeyEventArgs e)
  {
    if (e.Key == Key.Enter && _viewModel != null)
    {
      if (_viewModel.AddTaskCommand.CanExecute(null))
      {
        _viewModel.AddTaskCommand.Execute(null);
      }
      e.Handled = true;
    }
  }

  private void OnTaskCheckBoxClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
  {
    if (sender is CheckBox checkBox && checkBox.DataContext is TaskItem task && _viewModel != null)
    {
      // Просто вызываем метод напрямую вместо использования команды
      _viewModel.ExecuteToggleTaskCompletion(task);
    }
  }

  private void OnDeleteTaskClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
  {
    if (sender is Button button && button.DataContext is TaskItem task && _viewModel != null)
    {
      // Просто вызываем метод напрямую вместо использования команды
      _viewModel.ExecuteDeleteTask(task);
    }
  }
}
public class BoolToColorConverter : IValueConverter
{
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    if (value is bool boolValue && boolValue)
    {
      // Если true - возвращаем серый цвет для выполненной задачи
      return new SolidColorBrush(Colors.Gray);
    }

    // Если false - возвращаем черный цвет (по умолчанию)
    return new SolidColorBrush(Colors.Black);
  }

  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}