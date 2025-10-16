using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaApplication1.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
        Database.EnsureCreated();
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Movie> Movies { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Basket> Baskets { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Fifffflms;Username=postgres;Password=12345");
        }
    }
    public void CreateOrderTablesIfNotExist()
    {
        try
        {
            // Проверяем существование таблицы Orders
            var connection = Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            // Создаем таблицу Orders если не существует
            var createOrdersCommand = connection.CreateCommand();
            createOrdersCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS ""Orders"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""UserId"" INTEGER NOT NULL,
                    ""OrderDate"" TIMESTAMP WITH TIME ZONE NOT NULL,
                    ""Status"" TEXT
                )";
            createOrdersCommand.ExecuteNonQuery();

            // Создаем таблицу OrderItems если не существует
            var createOrderItemsCommand = connection.CreateCommand();
            createOrderItemsCommand.CommandText = @"
                CREATE TABLE IF NOT EXISTS ""OrderItems"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""OrderId"" INTEGER NOT NULL,
                    ""MovieId"" INTEGER NOT NULL,
                    ""Quantity"" INTEGER NOT NULL
                )";
            createOrderItemsCommand.ExecuteNonQuery();

            Console.WriteLine("Order tables created successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating order tables: {ex.Message}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Orders");
        });
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("OrderItems");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Movies");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Genre).HasMaxLength(100);
            entity.Property(e => e.Director).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Users");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
        });
    modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Baskets");

            entity.Property(e => e.AddedDate)
            .HasColumnType("timestamp with time zone");
               

            entity.HasOne(b => b.User)
                  .WithMany()
                  .HasForeignKey(b => b.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(b => b.Movie)
                  .WithMany()
                  .HasForeignKey(b => b.MovieId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(b => new { b.UserId, b.MovieId })
                  .IsUnique();
        });

        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}