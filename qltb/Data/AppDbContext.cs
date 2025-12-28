using Microsoft.EntityFrameworkCore;
using qltb.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace qltb.Data
{
    public class AppDbContext : DbContext
    {
        private static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "QLTB", "qltb.db");

        public DbSet<User> Users { get; set; }
        public DbSet<EquipmentItem> EquipmentItems { get; set; }
        public DbSet<IssueHistory> IssueHistories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Đảm bảo thư mục tồn tại
            Directory.CreateDirectory(Path.GetDirectoryName(DbPath));

            options.UseSqlite($"Data Source={DbPath}")
                   .EnableSensitiveDataLogging(false) // Tắt log sensitive data
                   .EnableDetailedErrors(true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });

            // EquipmentItem
            modelBuilder.Entity<EquipmentItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.QRCode).IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("datetime('now')");

                entity.OwnsOne(e => e.Spec, spec =>
                {
                    spec.Property(s => s.Caliber).HasMaxLength(50);
                    spec.Property(s => s.Weight).HasMaxLength(50);
                    spec.Property(s => s.Range).HasMaxLength(50);
                    spec.Property(s => s.Material).HasMaxLength(100);
                });

                entity.HasMany(e => e.TacticalFeatures)
                      .WithOne()
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // IssueHistory
            modelBuilder.Entity<IssueHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.EquipmentItemId);
                entity.Property(e => e.IssueDate).HasDefaultValueSql("datetime('now')");
            });

            // AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Time);
                entity.Property(e => e.Time).HasDefaultValueSql("datetime('now')");
            });

            // TacticalFeature
            modelBuilder.Entity<TacticalFeature>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is EquipmentItem &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (EquipmentItem)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = DateTime.Now;
                }
            }
        }
    }
}