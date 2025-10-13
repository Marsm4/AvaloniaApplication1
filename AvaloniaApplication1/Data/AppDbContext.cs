using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaApplication1.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TestDb;Username=postgres;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("movie_pkey");

            entity.ToTable("movie");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Director)
                .HasMaxLength(255)
                .HasColumnName("director");
            entity.Property(e => e.Genre)
                .HasMaxLength(100)
                .HasColumnName("genre");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
