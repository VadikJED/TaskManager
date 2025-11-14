using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Desktop.Data;
using TaskManager.Desktop.Models;
using TaskManager.Desktop.Repositories;

namespace TaskManager.Tests.Repositories;

public class TaskRepositoryTests : IDisposable
{
  private readonly ApplicationDbContext _context;
  private readonly Mock<ILogger<TaskRepository>> _mockLogger;
  private readonly TaskRepository _repository;

  public TaskRepositoryTests()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}")
      .Options;

    _context = new ApplicationDbContext(options);
    _mockLogger = new Mock<ILogger<TaskRepository>>();
    _repository = new TaskRepository(_context, _mockLogger.Object);

    // Ensure database is created
    _context.Database.EnsureCreated();
  }

  public void Dispose()
  {
    _context?.Database?.EnsureDeleted();
    _context?.Dispose();
  }

  [Fact]
  public async Task AddAsync_ShouldAddTaskToDatabase()
  {
    // Arrange
    var task = new TaskItem
    {
      Title = "Test Task",
      IsCompleted = false
    };

    // Act
    var result = await _repository.AddAsync(task);

    // Assert
    Assert.NotNull(result);
    Assert.NotEqual(Guid.Empty, result.Id);
    Assert.Equal("Test Task", result.Title);
    Assert.False(result.IsCompleted);
    Assert.True(result.CreatedAt <= DateTime.UtcNow);

    // Verify task is actually in database
    var tasksInDb = await _context.Tasks.ToListAsync();
    Assert.Single(tasksInDb);
    Assert.Equal(result.Id, tasksInDb[0].Id);
  }

  [Fact]
  public async Task GetAllAsync_ShouldReturnAllTasksOrderedByCreatedDate()
  {
    // Arrange
    var task1 = new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = "First Task",
      IsCompleted = false,
      CreatedAt = DateTime.UtcNow.AddHours(-2)
    };
    var task2 = new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = "Second Task",
      IsCompleted = true,
      CreatedAt = DateTime.UtcNow.AddHours(-1)
    };

    _context.Tasks.Add(task1);
    _context.Tasks.Add(task2);
    await _context.SaveChangesAsync();

    // Act
    var result = await _repository.GetAllAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Count);
    // Should be ordered by CreatedAt descending (newest first)
    Assert.Equal("Second Task", result[0].Title);
    Assert.Equal("First Task", result[1].Title);
  }

  [Fact]
  public async Task GetByIdAsync_WithExistingId_ShouldReturnTask()
  {
    // Arrange
    var taskId = Guid.NewGuid();
    var task = new TaskItem
    {
      Id = taskId,
      Title = "Test Task",
      IsCompleted = false
    };
    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();

    // Act
    var result = await _repository.GetByIdAsync(taskId);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(taskId, result.Id);
    Assert.Equal("Test Task", result.Title);
  }

  [Fact]
  public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
  {
    // Arrange
    var nonExistingId = Guid.NewGuid();

    // Act
    var result = await _repository.GetByIdAsync(nonExistingId);

    // Assert
    Assert.Null(result);
  }

  [Fact]
  public async Task UpdateAsync_ShouldUpdateExistingTask()
  {
    // Arrange
    var taskId = Guid.NewGuid();
    var task = new TaskItem
    {
      Id = taskId,
      Title = "Original Title",
      IsCompleted = false
    };
    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();

    // Detach the entity to simulate real-world scenario
    _context.Entry(task).State = EntityState.Detached;

    // Create updated task with same ID
    var updatedTask = new TaskItem
    {
      Id = taskId,
      Title = "Updated Title",
      IsCompleted = true,
      CreatedAt = task.CreatedAt
    };

    // Act
    var result = await _repository.UpdateAsync(updatedTask);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("Updated Title", result.Title);
    Assert.True(result.IsCompleted);

    // Verify changes persisted
    var taskFromDb = await _context.Tasks.FindAsync(taskId);
    Assert.NotNull(taskFromDb);
    Assert.Equal("Updated Title", taskFromDb.Title);
    Assert.True(taskFromDb.IsCompleted);
  }

  [Fact]
  public async Task DeleteAsync_WithExistingId_ShouldRemoveTask()
  {
    // Arrange
    var taskId = Guid.NewGuid();
    var task = new TaskItem
    {
      Id = taskId,
      Title = "Task to delete",
      IsCompleted = false
    };
    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();

    // Act
    var result = await _repository.DeleteAsync(taskId);

    // Assert
    Assert.True(result);

    var tasksInDb = await _context.Tasks.ToListAsync();
    Assert.Empty(tasksInDb);
  }

  [Fact]
  public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
  {
    // Arrange
    var nonExistingId = Guid.NewGuid();

    // Act
    var result = await _repository.DeleteAsync(nonExistingId);

    // Assert
    Assert.False(result);
  }

  [Fact]
  public async Task GetPendingAsync_ShouldReturnOnlyPendingTasks()
  {
    // Arrange
    var pendingTask = new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = "Pending Task",
      IsCompleted = false
    };
    var completedTask = new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = "Completed Task",
      IsCompleted = true
    };

    _context.Tasks.Add(pendingTask);
    _context.Tasks.Add(completedTask);
    await _context.SaveChangesAsync();

    // Act
    var result = await _repository.GetPendingAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal("Pending Task", result[0].Title);
    Assert.False(result[0].IsCompleted);
  }

  [Fact]
  public async Task GetCompletedAsync_ShouldReturnOnlyCompletedTasks()
  {
    // Arrange
    var pendingTask = new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = "Pending Task",
      IsCompleted = false
    };
    var completedTask = new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = "Completed Task",
      IsCompleted = true
    };

    _context.Tasks.Add(pendingTask);
    _context.Tasks.Add(completedTask);
    await _context.SaveChangesAsync();

    // Act
    var result = await _repository.GetCompletedAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal("Completed Task", result[0].Title);
    Assert.True(result[0].IsCompleted);
  }
}