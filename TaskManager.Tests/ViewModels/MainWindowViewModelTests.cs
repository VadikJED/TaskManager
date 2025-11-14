using Moq;
using TaskManager.Desktop.Repositories;
using TaskManager.Desktop.ViewModels;
using TaskManager.Desktop.Models;
using Microsoft.Extensions.Logging;
using Xunit;

namespace TaskManager.Tests.ViewModels;

public class MainWindowViewModelTests
{
  private readonly Mock<ITaskRepository> _mockRepository;
  private readonly Mock<ILogger<MainWindowViewModel>> _mockLogger;
  private readonly MainWindowViewModel _viewModel;

  public MainWindowViewModelTests()
  {
    _mockRepository = new Mock<ITaskRepository>();
    _mockLogger = new Mock<ILogger<MainWindowViewModel>>();
    _viewModel = new MainWindowViewModel(_mockRepository.Object, _mockLogger.Object);
  }

  [Fact]
  public async Task AddTaskAsync_WhenTitleIsEmpty_ShouldNotSaveToRepository()
  {
    // Arrange
    _viewModel.NewTaskTitle = "";

    // Act
    await _viewModel.AddTaskCommand.ExecuteAsync(null);

    // Assert
    _mockRepository.Verify(
        repo => repo.AddAsync(It.IsAny<TaskItem>()),
        Times.Never,
        "Repository should not be called when title is empty"
    );
  }

  [Fact]
  public async Task AddTaskAsync_WhenTitleIsWhitespace_ShouldNotSaveToRepository()
  {
    // Arrange
    _viewModel.NewTaskTitle = "   ";

    // Act
    await _viewModel.AddTaskCommand.ExecuteAsync(null);

    // Assert
    _mockRepository.Verify(
        repo => repo.AddAsync(It.IsAny<TaskItem>()),
        Times.Never,
        "Repository should not be called when title is whitespace"
    );
  }

  [Fact]
  public async Task AddTaskAsync_WhenTitleIsNull_ShouldNotSaveToRepository()
  {
    // Arrange
    _viewModel.NewTaskTitle = null;

    // Act
    await _viewModel.AddTaskCommand.ExecuteAsync(null);

    // Assert
    _mockRepository.Verify(
        repo => repo.AddAsync(It.IsAny<TaskItem>()),
        Times.Never,
        "Repository should not be called when title is null"
    );
  }

  [Fact]
  public async Task AddTaskAsync_WhenTitleIsValid_ShouldSaveToRepository()
  {
    // Arrange
    var expectedTitle = "Test Task";
    _viewModel.NewTaskTitle = expectedTitle;

    var createdTask = new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = expectedTitle,
      IsCompleted = false,
      CreatedAt = DateTime.UtcNow
    };

    _mockRepository
        .Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()))
        .ReturnsAsync(createdTask);

    // Act
    await _viewModel.AddTaskCommand.ExecuteAsync(null);

    // Assert
    _mockRepository.Verify(
        repo => repo.AddAsync(It.IsAny<TaskItem>()),
        Times.Once,
        "Repository should be called exactly once when title is valid"
    );

    _mockRepository.Verify(
        repo => repo.AddAsync(It.Is<TaskItem>(task =>
            task.Title == expectedTitle &&
            !task.IsCompleted)),
        Times.Once,
        "Repository should receive task with correct title and completion status"
    );
  }

  [Fact]
  public async Task AddTaskAsync_WhenSuccessful_ShouldClearTitleAndUpdateCollections()
  {
    // Arrange
    var testTitle = "Test Task";
    _viewModel.NewTaskTitle = testTitle;

    var createdTask = new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = testTitle,
      IsCompleted = false,
      CreatedAt = DateTime.UtcNow
    };

    _mockRepository
        .Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()))
        .ReturnsAsync(createdTask);

    // Act
    await _viewModel.AddTaskCommand.ExecuteAsync(null);

    // Assert
    Assert.Equal(string.Empty, _viewModel.NewTaskTitle);
    Assert.Single(_viewModel.AllTasks);
    Assert.Single(_viewModel.PendingTasks);
    Assert.Empty(_viewModel.CompletedTasks);

    var addedTask = _viewModel.AllTasks.First();
    Assert.Equal(testTitle, addedTask.Title);
    Assert.False(addedTask.IsCompleted);
  }

  [Fact]
  public async Task AddTaskAsync_WhenRepositoryThrowsException_ShouldUpdateStatusMessage()
  {
    // Arrange
    var testTitle = "Test Task";
    _viewModel.NewTaskTitle = testTitle;

    var exceptionMessage = "Database connection failed";
    _mockRepository
        .Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()))
        .ThrowsAsync(new Exception(exceptionMessage));

    // Act
    await _viewModel.AddTaskCommand.ExecuteAsync(null);

    // Assert
    Assert.Contains(exceptionMessage, _viewModel.StatusMessage);
    Assert.Empty(_viewModel.AllTasks);
  }
}


