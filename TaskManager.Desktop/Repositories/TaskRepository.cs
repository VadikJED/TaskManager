using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Desktop.Data;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Repositories;

public class TaskRepository : ITaskRepository
{
  private readonly ApplicationDbContext _context;
  private readonly ILogger<TaskRepository> _logger;

  public TaskRepository(ApplicationDbContext context, ILogger<TaskRepository> logger)
  {
    _context = context;
    _logger = logger;

    _logger.LogInformation("TaskRepository initialized");
  }

  public async Task<List<TaskItem>> GetAllAsync()
  {
    _logger.LogInformation("Getting all tasks");

    try
    {
      var tasks = await _context.Tasks
          .OrderByDescending(t => t.CreatedAt)
          .ToListAsync();

      _logger.LogInformation("Retrieved {Count} tasks from database", tasks.Count);
      return tasks;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while getting all tasks");
      throw;
    }
  }

  public async Task<TaskItem?> GetByIdAsync(Guid id)
  {
    _logger.LogInformation("Getting task by ID: {TaskId}", id);

    try
    {
      var task = await _context.Tasks.FindAsync(id);

      if (task == null)
      {
        _logger.LogWarning("Task with ID {TaskId} not found", id);
      }
      else
      {
        _logger.LogDebug("Task found: {TaskTitle}", task.Title);
      }

      return task;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while getting task by ID: {TaskId}", id);
      throw;
    }
  }

  public async Task<TaskItem> AddAsync(TaskItem task)
  {
    _logger.LogInformation("Adding new task: {TaskTitle}", task.Title);

    try
    {
      task.Id = Guid.NewGuid();
      task.CreatedAt = DateTime.UtcNow;

      _context.Tasks.Add(task);
      await _context.SaveChangesAsync();

      _logger.LogInformation("Task added successfully with ID: {TaskId}", task.Id);
      return task;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while adding task: {TaskTitle}", task.Title);
      throw;
    }
  }

  public async Task<TaskItem> UpdateAsync(TaskItem task)
  {
    _logger.LogInformation("Updating task: {TaskId} - {TaskTitle}", task.Id, task.Title);

    try
    {
      _context.Entry(task).State = EntityState.Modified;
      await _context.SaveChangesAsync();

      _logger.LogInformation("Task updated successfully: {TaskId}", task.Id);
      return task;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while updating task: {TaskId}", task.Id);
      throw;
    }
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    _logger.LogInformation("Deleting task with ID: {TaskId}", id);

    try
    {
      var task = await _context.Tasks.FindAsync(id);
      if (task != null)
      {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Task deleted successfully: {TaskId}", id);
        return true;
      }

      _logger.LogWarning("Task not found for deletion: {TaskId}", id);
      return false;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while deleting task: {TaskId}", id);
      throw;
    }
  }

  public async Task<List<TaskItem>> GetCompletedAsync()
  {
    _logger.LogDebug("Getting completed tasks");

    try
    {
      var tasks = await _context.Tasks
          .Where(t => t.IsCompleted)
          .OrderByDescending(t => t.CreatedAt)
          .ToListAsync();

      _logger.LogDebug("Retrieved {Count} completed tasks", tasks.Count);
      return tasks;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while getting completed tasks");
      throw;
    }
  }

  public async Task<List<TaskItem>> GetPendingAsync()
  {
    _logger.LogDebug("Getting pending tasks");

    try
    {
      var tasks = await _context.Tasks
          .Where(t => !t.IsCompleted)
          .OrderByDescending(t => t.CreatedAt)
          .ToListAsync();

      _logger.LogDebug("Retrieved {Count} pending tasks", tasks.Count);
      return tasks;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error occurred while getting pending tasks");
      throw;
    }
  }
}