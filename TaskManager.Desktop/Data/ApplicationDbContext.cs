using Microsoft.EntityFrameworkCore;
using TaskManager.Desktop.Models;
using TaskManager.Desktop.Data.Configurations;

namespace TaskManager.Desktop.Data;

public class ApplicationDbContext : DbContext
{
  // Основной конструктор для Dependency Injection
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  // Конструктор без параметров для тестов и миграций
  public ApplicationDbContext()
  {
  }

  public DbSet<TaskItem> Tasks { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    // Этот метод будет использоваться только если контекст не сконфигурирован через DI
    if (!optionsBuilder.IsConfigured)
      // Для миграций используем PostgreSQL
      optionsBuilder.UseNpgsql("Host=localhost;Database=taskmanager;Username=postgres;Password=1");
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new TaskItemConfiguration());
    base.OnModelCreating(modelBuilder);
  }
}