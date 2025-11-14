using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManager.Desktop.Models;
using TaskManager.Desktop.Data;

namespace TaskManager.Desktop.Repositories;

public class TaskRepository : ITaskRepository
{
  private readonly ApplicationDbContext _context;

  public TaskRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<List<TaskItem>> GetAllAsync()
  {
    return await _context.Tasks
      .OrderByDescending(t => t.CreatedAt)
      .ToListAsync();
  }

  public async Task<TaskItem?> GetByIdAsync(Guid id)
  {
    return await _context.Tasks.FindAsync(id);
  }

  public async Task<TaskItem> AddAsync(TaskItem task)
  {
    task.Id = Guid.NewGuid();
    task.CreatedAt = DateTime.UtcNow;

    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();

    return task;
  }

  public async Task<TaskItem> UpdateAsync(TaskItem task)
  {
    await _context.SaveChangesAsync();
    return task;
  }

  public async Task<bool> DeleteAsync(Guid id)
  {
    var task = await _context.Tasks.FindAsync(id);
    if (task != null)
    {
      _context.Tasks.Remove(task);
      await _context.SaveChangesAsync();
      return true;
    }
    return false;
  }

  public async Task<List<TaskItem>> GetCompletedAsync()
  {
    return await _context.Tasks
      .Where(t => t.IsCompleted)
      .OrderByDescending(t => t.CreatedAt)
      .ToListAsync();
  }

  public async Task<List<TaskItem>> GetPendingAsync()
  {
    return await _context.Tasks
      .Where(t => !t.IsCompleted)
      .OrderByDescending(t => t.CreatedAt)
      .ToListAsync();
  }
}