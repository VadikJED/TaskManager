using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Repositories;


public interface ITaskRepository
{
  Task<List<TaskItem>> GetAllAsync();
  Task<TaskItem?> GetByIdAsync(Guid id);
  Task<TaskItem> AddAsync(TaskItem task);
  Task<TaskItem> UpdateAsync(TaskItem task);
  Task<bool> DeleteAsync(Guid id);
  Task<List<TaskItem>> GetCompletedAsync();
  Task<List<TaskItem>> GetPendingAsync();
}