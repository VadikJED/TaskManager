using CommunityToolkit.Mvvm.ComponentModel;
using System;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.ViewModels;
public partial class TaskItemViewModel : ObservableObject
{
  [ObservableProperty]
  private Guid _id;

  [ObservableProperty]
  private string _title = string.Empty;

  [ObservableProperty]
  private bool _isCompleted;

  [ObservableProperty]
  private DateTime _createdAt;

  public TaskItemViewModel() { }

  public TaskItemViewModel(TaskItem task)
  {
    Id = task.Id;
    Title = task.Title;
    IsCompleted = task.IsCompleted;
    CreatedAt = task.CreatedAt;
  }

  public TaskItem ToModel()
  {
    return new TaskItem
    {
      Id = Id,
      Title = Title,
      IsCompleted = IsCompleted,
      CreatedAt = CreatedAt
    };
  }

  public void UpdateFromModel(TaskItem task)
  {
    Id = task.Id;
    Title = task.Title;
    IsCompleted = task.IsCompleted;
    CreatedAt = task.CreatedAt;
  }
}