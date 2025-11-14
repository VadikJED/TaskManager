using Microsoft.EntityFrameworkCore;
using TaskManager.Desktop.Models;
using TaskManager.Desktop.Data.Configurations;

namespace TaskManager.Desktop.Data;
public class ApplicationDbContext : DbContext
{
  public DbSet<TaskItem> Tasks { get; set; }

  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
  {
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      // Для миграций и DesignTime
      optionsBuilder.UseNpgsql("Host=localhost;Database=taskmanager;Username=postgres;Password=1");
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // Применяем все конфигурации из сборки
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
  }
}

