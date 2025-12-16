using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace backend_onboarding.Models.Entities.DbOnboardingRIMS;

public partial class OnboardingRimsContext : DbContext
{
    public OnboardingRimsContext()
    {
    }

    public OnboardingRimsContext(DbContextOptions<OnboardingRimsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Adaccount> Adaccounts { get; set; }

    public virtual DbSet<Email> Emails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=OnboardingRIMS;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Adaccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ADAccoun__3213E83FFD853DC4");

            entity.ToTable("ADAccount");

            entity.HasIndex(e => e.Id, "UQ__ADAccoun__3213E83E3B0EDB3B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountName)
                .HasMaxLength(255)
                .HasColumnName("account_name");
            entity.Property(e => e.AppName)
                .HasMaxLength(255)
                .HasColumnName("app_name");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Adaccounts)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ADAccount__FK_us__2C3393D0");
        });

        modelBuilder.Entity<Email>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Email__3213E83FF28ADB09");

            entity.ToTable("Email");

            entity.HasIndex(e => e.Id, "UQ__Email__3213E83E32795AF4").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email1)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FkUserId).HasColumnName("FK_user_id");

            entity.HasOne(d => d.FkUser).WithMany(p => p.Emails)
                .HasForeignKey(d => d.FkUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Email__FK_user_i__2D27B809");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83F6F35849D");

            entity.ToTable("User");

            entity.HasIndex(e => e.Id, "UQ__User__3213E83EACB0D673").IsUnique();

            entity.HasIndex(e => e.Uid, "UQ__User__DD70126501FA4E47").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Caption)
                .HasMaxLength(255)
                .HasColumnName("caption");
            entity.Property(e => e.Company)
                .HasMaxLength(255)
                .HasColumnName("company");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasColumnName("department");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(255)
                .HasColumnName("job_title");
            entity.Property(e => e.Uid).HasColumnName("uid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
