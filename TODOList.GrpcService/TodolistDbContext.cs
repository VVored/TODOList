using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TODOList.GrpcService;

public partial class TodolistDbContext : DbContext
{
    public TodolistDbContext()
    {
    }

    public TodolistDbContext(DbContextOptions<TodolistDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Doing> Doings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doing>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doings_pk");

            entity.ToTable("doings");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("added_date");
            entity.Property(e => e.CompletionDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("completion_date");
            entity.Property(e => e.Iscomplete)
                .HasDefaultValue(false)
                .HasColumnName("iscomplete");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
