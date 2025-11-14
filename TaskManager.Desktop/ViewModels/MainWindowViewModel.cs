using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Desktop.Models;
using TaskManager.Desktop.Repositories;

namespace TaskManager.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
  private readonly ITaskRepository _taskRepository;

  [ObservableProperty]
  private string newTaskTitle = string.Empty;

  [ObservableProperty]
  private bool isLoading;

  [ObservableProperty]
  private string statusMessage = "Готов";

  public ObservableCollection<TaskItemViewModel> AllTasks { get; } = new();
  public ObservableCollection<TaskItemViewModel> PendingTasks { get; } = new();
  public ObservableCollection<TaskItemViewModel> CompletedTasks { get; } = new();

  public MainWindowViewModel(ITaskRepository taskRepository)
  {
    _taskRepository = taskRepository;
    _ = LoadTasksAsync();
  }

  [RelayCommand]
  private async Task LoadTasksAsync()
  {
    IsLoading = true;
    StatusMessage = "Загрузка задач...";

    try
    {
      var tasks = await _taskRepository.GetAllAsync();

      AllTasks.Clear();
      PendingTasks.Clear();
      CompletedTasks.Clear();

      foreach (var task in tasks)
      {
        var taskVM = new TaskItemViewModel(task);
        AllTasks.Add(taskVM);

        if (task.IsCompleted)
          CompletedTasks.Add(taskVM);
        else
          PendingTasks.Add(taskVM);
      }

      StatusMessage = $"Загружено {AllTasks.Count} задач";
    }
    catch (Exception ex)
    {
      StatusMessage = $"Ошибка загрузки: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  [RelayCommand]
  private async Task AddTaskAsync()
  {
    if (string.IsNullOrWhiteSpace(NewTaskTitle))
    {
      StatusMessage = "Введите название задачи";
      return;
    }

    try
    {
      var newTask = new TaskItem
      {
        Title = NewTaskTitle.Trim(),
        IsCompleted = false
      };

      var createdTask = await _taskRepository.AddAsync(newTask);
      var taskVM = new TaskItemViewModel(createdTask);

      AllTasks.Insert(0, taskVM);
      PendingTasks.Insert(0, taskVM);

      NewTaskTitle = string.Empty;
      StatusMessage = "Задача добавлена";
    }
    catch (Exception ex)
    {
      StatusMessage = $"Ошибка добавления: {ex.Message}";
    }
  }

  [RelayCommand]
  private async Task UpdateTaskAsync(TaskItemViewModel taskVM)
  {
    try
    {
      var task = taskVM.ToModel();
      await _taskRepository.UpdateAsync(task);

      // Обновляем списки
      PendingTasks.Clear();
      CompletedTasks.Clear();

      foreach (var item in AllTasks)
      {
        if (item.IsCompleted)
          CompletedTasks.Add(item);
        else
          PendingTasks.Add(item);
      }

      StatusMessage = "Задача обновлена";
    }
    catch (Exception ex)
    {
      StatusMessage = $"Ошибка обновления: {ex.Message}";
    }
  }

  [RelayCommand]
  private async Task DeleteTaskAsync(TaskItemViewModel task)
  {
    try
    {
      var success = await _taskRepository.DeleteAsync(task.Id);
      if (success)
      {
        AllTasks.Remove(task);
        PendingTasks.Remove(task);
        CompletedTasks.Remove(task);
        StatusMessage = "Задача удалена";
      }
    }
    catch (Exception ex)
    {
      StatusMessage = $"Ошибка удаления: {ex.Message}";
    }
  }

  [RelayCommand]
  private async Task ToggleTaskCompletionAsync(TaskItemViewModel task)
  {
    task.IsCompleted = !task.IsCompleted;
    await UpdateTaskAsync(task);
  }

  [RelayCommand]
  private async Task ClearCompletedTasksAsync()
  {
    try
    {
      var completedTasks = CompletedTasks.ToList();
      foreach (var task in completedTasks)
      {
        await _taskRepository.DeleteAsync(task.Id);
        AllTasks.Remove(task);
      }
      CompletedTasks.Clear();
      StatusMessage = $"Удалено {completedTasks.Count} завершенных задач";
    }
    catch (Exception ex)
    {
      StatusMessage = $"Ошибка очистки: {ex.Message}";
    }
  }
}