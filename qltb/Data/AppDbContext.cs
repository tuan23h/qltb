using Microsoft.EntityFrameworkCore;
using qltb.Models;

namespace qltb.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<EquipmentItem> EquipmentItems { get; set; }
        public DbSet<IssueHistory> IssueHistories { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=qltb.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EquipmentItem>()
                .OwnsOne(e => e.Spec);

            modelBuilder.Entity<EquipmentItem>()
                .HasMany(e => e.TacticalFeatures)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
