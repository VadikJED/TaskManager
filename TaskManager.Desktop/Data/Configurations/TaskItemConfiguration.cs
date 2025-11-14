using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Desktop.Models;

namespace TaskManager.Desktop.Data.Configurations;


public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
  public void Configure(EntityTypeBuilder<TaskItem> builder)
  {
    builder.ToTable("Tasks"); // Явно указываем имя таблицы

    builder.HasKey(t => t.Id);

    builder.Property(t => t.Id)
      .IsRequired()
      .ValueGeneratedOnAdd();

    builder.Property(t => t.Title)
      .IsRequired()
      .HasMaxLength(100);

    builder.Property(t => t.IsCompleted)
      .IsRequired();

    builder.Property(t => t.CreatedAt)
      .IsRequired();

    // Указываем точные имена колонок для PostgreSQL
    builder.Property(t => t.Id)
      .HasColumnName("Id");
    builder.Property(t => t.Title)
      .HasColumnName("Title");
    builder.Property(t => t.IsCompleted)
      .HasColumnName("IsCompleted");
    builder.Property(t => t.CreatedAt)
      .HasColumnName("CreatedAt");
  }
}