using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TaskManager.Models;

namespace TaskManager.Services;

public class TaskService
{
  private readonly List<TaskItem> _tasks = new();
  private readonly object _lock = new object();

  public TaskService()
  {
    // Add some sample tasks for demonstration
    _tasks.Add(new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = "Learn Avalonia UI",
      IsCompleted = false,
      CreatedAt = DateTime.UtcNow.AddHours(-2),
      IsDeleted = false
    });

    _tasks.Add(new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = "Create Task Manager",
      IsCompleted = true,
      CreatedAt = DateTime.UtcNow.AddHours(-1),
      IsDeleted = false
    });
  }

  public Task<List<TaskItem>> GetAllTasksAsync()
  {
    lock (_lock)
    {
      var activeTasks = _tasks
          .Where(t => !t.IsDeleted)
          .OrderByDescending(t => t.CreatedAt)
          .ToList();

      return Task.FromResult(activeTasks);
    }
  }

  public Task<Result<TaskItem>> AddTaskAsync(TaskItem task)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(task.Title))
        return Task.FromResult(Result<TaskItem>.Failure("Task title cannot be empty"));

      if (task.Title.Length > 100)
        return Task.FromResult(Result<TaskItem>.Failure("Task title cannot exceed 100 characters"));

      lock (_lock)
      {
        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.IsCompleted = false;
        task.IsDeleted = false;

        _tasks.Add(task);
      }

      return Task.FromResult(Result<TaskItem>.Success(task));
    }
    catch (Exception ex)
    {
      return Task.FromResult(Result<TaskItem>.Failure($"Error adding task: {ex.Message}"));
    }
  }

  public Task<Result> UpdateTaskAsync(TaskItem task)
  {
    try
    {
      if (string.IsNullOrWhiteSpace(task.Title))
        return Task.FromResult(Result.Failure("Task title cannot be empty"));

      if (task.Title.Length > 100)
        return Task.FromResult(Result.Failure("Task title cannot exceed 100 characters"));

      lock (_lock)
      {
        var existingTask = _tasks.FirstOrDefault(t => t.Id == task.Id);
        if (existingTask == null)
          return Task.FromResult(Result.Failure("Task not found"));

        existingTask.Title = task.Title;
        existingTask.IsCompleted = task.IsCompleted;
      }

      return Task.FromResult(Result.Success());
    }
    catch (Exception ex)
    {
      return Task.FromResult(Result.Failure($"Error updating task: {ex.Message}"));
    }
  }

  public Task<Result> DeleteTaskAsync(Guid id)
  {
    try
    {
      lock (_lock)
      {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task == null)
          return Task.FromResult(Result.Failure("Task not found"));

        task.IsDeleted = true;
      }

      return Task.FromResult(Result.Success());
    }
    catch (Exception ex)
    {
      return Task.FromResult(Result.Failure($"Error deleting task: {ex.Message}"));
    }
  }
}


public class Result
{
  public bool IsSuccess { get; set; }
  public string Error { get; set; } = string.Empty;

  public static Result Success() => new() { IsSuccess = true };
  public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
}

public class Result<T> : Result
{
  public T? Value { get; set; }

  public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
  public static new Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
}