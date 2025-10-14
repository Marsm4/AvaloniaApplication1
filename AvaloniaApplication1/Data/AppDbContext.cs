using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaApplication1.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
        // Гарантируем, что база данных создана
        Database.EnsureCreated();
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Movie> Movies { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Basket> Baskets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TestDb;Username=postgres;Password=123");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Конфигурация Movie
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Movies");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Genre).HasMaxLength(100);
            entity.Property(e => e.Director).HasMaxLength(255);
        });

        // Конфигурация User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Users");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        // Конфигурация Basket
        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Baskets");

            // Настройка DateTime для PostgreSQL
            entity.Property(e => e.AddedDate)
            .HasColumnType("timestamp with time zone"); // Для DateTimeOffset
               

            // Внешний ключ для User
            entity.HasOne(b => b.User)
                  .WithMany()
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Внешний ключ для Movie
            entity.HasOne(b => b.Movie)
                  .WithMany()
                  .HasForeignKey(b => b.MovieId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Уникальный индекс для предотвращения дубликатов
            entity.HasIndex(b => new { b.UserId, b.MovieId })
                  .IsUnique();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}