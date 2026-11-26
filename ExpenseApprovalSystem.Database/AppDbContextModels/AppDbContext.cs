using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExpenseApprovalSystem.Database.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAuditLog> TblAuditLogs { get; set; }

    public virtual DbSet<TblExpense> TblExpenses { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblAudit__3214EC072BC0FD7C");

            entity.ToTable("TblAuditLog");

            entity.Property(e => e.Id)
                .HasMaxLength(64)
                .HasDefaultValueSql("(CONVERT([nvarchar](64),newid()))");
            entity.Property(e => e.Action).HasMaxLength(32);
            entity.Property(e => e.ActionDateTime).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ActorRole).HasMaxLength(64);
            entity.Property(e => e.ActorUserId).HasMaxLength(64);
            entity.Property(e => e.ResourceType).HasMaxLength(64);
            entity.Property(e => e.TargetId).HasMaxLength(64);
            entity.Property(e => e.TargetLookupKey).HasMaxLength(256);
            entity.Property(e => e.TargetUserId).HasMaxLength(64);
            entity.Property(e => e.TargetUserMobileNo).HasMaxLength(32);
            entity.Property(e => e.TargetUserName).HasMaxLength(100);
        });

        modelBuilder.Entity<TblExpense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblExpen__3214EC0734790CA0");

            entity.ToTable("TblExpense");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.RequestedBy).HasMaxLength(100);
            entity.Property(e => e.ReviewedBy).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TblUser__3214EC07FE63D135");

            entity.ToTable("TblUser");

            entity.Property(e => e.Id).HasMaxLength(64);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Limit).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MobileNo).HasMaxLength(32);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(32)
                .HasDefaultValue("Pending");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
