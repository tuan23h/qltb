using Microsoft.EntityFrameworkCore;
using qltb.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace qltb.Data
{
    public class AppDbContext : DbContext
    {
        // SQL Server LocalDB connection string
        private static readonly string ConnectionString =
            @"Server=(localdb)\mssqllocaldb;Database=QLTB_DB;Integrated Security=true;MultipleActiveResultSets=true;";

        public DbSet<User> Users { get; set; }
        public DbSet<EquipmentItem> EquipmentItems { get; set; }
        public DbSet<IssueHistory> IssueHistories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(ConnectionString)
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
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FullName).HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });

            // EquipmentItem
            modelBuilder.Entity<EquipmentItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.QRCode).IsUnique();
                entity.Property(e => e.QRCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.CreatedBy).HasMaxLength(50);
                entity.Property(e => e.UpdatedBy).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.OwnsOne(e => e.Spec, spec =>
                {
                    spec.Property(s => s.Caliber).HasMaxLength(50).HasColumnName("Spec_Caliber");
                    spec.Property(s => s.Weight).HasMaxLength(50).HasColumnName("Spec_Weight");
                    spec.Property(s => s.Range).HasMaxLength(50).HasColumnName("Spec_Range");
                    spec.Property(s => s.Material).HasMaxLength(100).HasColumnName("Spec_Material");
                });

                entity.HasMany(e => e.TacticalFeatures)
                      .WithOne()
                      .HasForeignKey("EquipmentItemId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TacticalFeature
            modelBuilder.Entity<TacticalFeature>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            });

            // IssueHistory
            modelBuilder.Entity<IssueHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.EquipmentItemId);
                entity.Property(e => e.IssuedTo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Note).HasMaxLength(500);
                entity.Property(e => e.IssueDate).HasDefaultValueSql("GETDATE()");
            });

            // AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Time);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(200);
                entity.Property(e => e.User).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Time).HasDefaultValueSql("GETDATE()");
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