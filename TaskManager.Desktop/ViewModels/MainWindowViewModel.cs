using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
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
  private readonly ILogger<MainWindowViewModel> _logger;

  [ObservableProperty]
  private string _newTaskTitle = string.Empty;

  [ObservableProperty]
  private bool _isLoading;

  [ObservableProperty]
  private string _statusMessage = "Ready";

  public ObservableCollection<TaskItemViewModel> AllTasks { get; } = new();
  public ObservableCollection<TaskItemViewModel> PendingTasks { get; } = new();
  public ObservableCollection<TaskItemViewModel> CompletedTasks { get; } = new();

  public MainWindowViewModel(ITaskRepository taskRepository, ILogger<MainWindowViewModel> logger)
  {
    _taskRepository = taskRepository;
    _logger = logger;

    _logger.LogInformation("MainWindowViewModel initialized");
    _ = LoadTasksAsync();
  }

  [RelayCommand]
  private async Task LoadTasksAsync()
  {
    _logger.LogInformation("Loading tasks...");

    IsLoading = true;
    StatusMessage = "Loading tasks...";

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

      StatusMessage = $"Loaded {AllTasks.Count} tasks";
      _logger.LogInformation("Successfully loaded {TaskCount} tasks", AllTasks.Count);
    }
    catch (Exception ex)
    {
      StatusMessage = $"Error loading: {ex.Message}";
      _logger.LogError(ex, "Failed to load tasks");
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
      StatusMessage = "Please enter task title";
      _logger.LogWarning("Attempted to add task with empty title");
      return;
    }

    _logger.LogInformation("Adding new task: {TaskTitle}", NewTaskTitle);

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
      StatusMessage = "Task added successfully";

      _logger.LogInformation("Task added successfully. Total tasks: {TaskCount}", AllTasks.Count);
    }
    catch (Exception ex)
    {
      StatusMessage = $"Error adding task: {ex.Message}";
      _logger.LogError(ex, "Failed to add task: {TaskTitle}", NewTaskTitle);
    }
  }

  [RelayCommand]
  private async Task UpdateTaskAsync(TaskItemViewModel taskVM)
  {
    _logger.LogDebug("Updating task: {TaskId} - {TaskTitle}", taskVM.Id, taskVM.Title);

    try
    {
      var task = taskVM.ToModel();
      await _taskRepository.UpdateAsync(task);

      // Update filtered lists
      PendingTasks.Clear();
      CompletedTasks.Clear();

      foreach (var item in AllTasks)
      {
        if (item.IsCompleted)
          CompletedTasks.Add(item);
        else
          PendingTasks.Add(item);
      }

      StatusMessage = "Task updated";
      _logger.LogDebug("Task updated successfully: {TaskId}", taskVM.Id);
    }
    catch (Exception ex)
    {
      StatusMessage = $"Error updating: {ex.Message}";
      _logger.LogError(ex, "Failed to update task: {TaskId}", taskVM.Id);
    }
  }

  [RelayCommand]
  private async Task DeleteTaskAsync(TaskItemViewModel task)
  {
    _logger.LogInformation("Deleting task: {TaskId} - {TaskTitle}", task.Id, task.Title);

    try
    {
      var success = await _taskRepository.DeleteAsync(task.Id);
      if (success)
      {
        AllTasks.Remove(task);
        PendingTasks.Remove(task);
        CompletedTasks.Remove(task);
        StatusMessage = "Task deleted";

        _logger.LogInformation("Task deleted successfully: {TaskId}", task.Id);
      }
      else
      {
        _logger.LogWarning("Task not found for deletion: {TaskId}", task.Id);
      }
    }
    catch (Exception ex)
    {
      StatusMessage = $"Error deleting: {ex.Message}";
      _logger.LogError(ex, "Failed to delete task: {TaskId}", task.Id);
    }
  }

  [RelayCommand]
  private async Task ToggleTaskCompletionAsync(TaskItemViewModel task)
  {
    var newStatus = !task.IsCompleted;
    _logger.LogInformation("Toggling task completion: {TaskId} from {OldStatus} to {NewStatus}",
        task.Id, task.IsCompleted, newStatus);

    task.IsCompleted = newStatus;
    await UpdateTaskAsync(task);
  }

  [RelayCommand]
  private async Task ClearCompletedTasksAsync()
  {
    _logger.LogInformation("Clearing {Count} completed tasks", CompletedTasks.Count);

    try
    {
      var completedTasks = CompletedTasks.ToList();
      foreach (var task in completedTasks)
      {
        await _taskRepository.DeleteAsync(task.Id);
        AllTasks.Remove(task);
      }
      CompletedTasks.Clear();
      StatusMessage = $"Cleared {completedTasks.Count} completed tasks";

      _logger.LogInformation("Successfully cleared {Count} completed tasks", completedTasks.Count);
    }
    catch (Exception ex)
    {
      StatusMessage = $"Error clearing: {ex.Message}";
      _logger.LogError(ex, "Failed to clear completed tasks");
    }
  }
}