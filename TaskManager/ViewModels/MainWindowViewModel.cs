using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManager.Common;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.ViewModels;

public class MainViewModel : ViewModelBase
{
  private readonly TaskService _taskService;
  private string _newTaskTitle = string.Empty;
  private string _errorMessage = string.Empty;

  public MainViewModel()
  {
    _taskService = new TaskService();
    Tasks = new ObservableCollection<TaskItem>();

    AddTaskCommand = new RelayCommand(AddTask, () => !string.IsNullOrWhiteSpace(NewTaskTitle));
    LoadTasksCommand = new RelayCommand(LoadTasks);

    // Load tasks on startup
    LoadTasksCommand.Execute(null);
  }

  public ObservableCollection<TaskItem> Tasks { get; }

  public string NewTaskTitle
  {
    get => _newTaskTitle;
    set
    {
      _newTaskTitle = value;
      OnPropertyChanged();
      (AddTaskCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }
  }

  public string ErrorMessage
  {
    get => _errorMessage;
    set
    {
      _errorMessage = value;
      OnPropertyChanged();
    }
  }

  public ICommand AddTaskCommand { get; }
  public ICommand LoadTasksCommand { get; }

  // Public methods to call from View
  public async void ExecuteToggleTaskCompletion(TaskItem task)
  {
    if (task == null) return;

    try
    {
      ClearError();

      task.IsCompleted = !task.IsCompleted;
      var result = await _taskService.UpdateTaskAsync(task);

      if (!result.IsSuccess)
      {
        // Revert the change if update failed
        task.IsCompleted = !task.IsCompleted;
        ShowError(result.Error);
      }
    }
    catch (Exception ex)
    {
      ShowError($"Failed to update task: {ex.Message}");
    }
  }

  public async void ExecuteDeleteTask(TaskItem task)
  {
    if (task == null) return;

    try
    {
      ClearError();

      var result = await _taskService.DeleteTaskAsync(task.Id);

      if (result.IsSuccess)
      {
        Tasks.Remove(task);
      }
      else
      {
        ShowError(result.Error);
      }
    }
    catch (Exception ex)
    {
      ShowError($"Failed to delete task: {ex.Message}");
    }
  }

  private void AddTask()
  {
    try
    {
      ClearError();

      var newTask = new TaskItem { Title = NewTaskTitle.Trim() };

      var result = _taskService.AddTaskAsync(newTask).Result;

      if (result.IsSuccess && result.Value != null)
      {
        Tasks.Insert(0, result.Value);
        NewTaskTitle = string.Empty;
      }
      else
      {
        ShowError(result.Error);
      }
    }
    catch (Exception ex)
    {
      ShowError($"Failed to add task: {ex.Message}");
    }
  }

  private void LoadTasks()
  {
    try
    {
      ClearError();

      var taskList = _taskService.GetAllTasksAsync().Result;
      Tasks.Clear();

      foreach (var task in taskList)
      {
        Tasks.Add(task);
      }
    }
    catch (Exception ex)
    {
      ShowError($"Failed to load tasks: {ex.Message}");
    }
  }

  private void ShowError(string message)
  {
    ErrorMessage = message;
  }

  private void ClearError()
  {
    ErrorMessage = string.Empty;
  }
}