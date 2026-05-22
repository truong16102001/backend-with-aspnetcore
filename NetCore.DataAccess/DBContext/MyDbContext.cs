using Microsoft.EntityFrameworkCore;
using NetCore.DataAccess.DataObject.Entities;

namespace NetCore.DataAccess.DBContext
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(
            DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        // =========================
        // DbSets
        // =========================

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Feature> Features { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<UserPermission> UserPermissions { get; set; }

        // =========================
        // Fluent API Configuration
        // =========================

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================
            // UserPermission -> User
            // 1 User has many UserPermissions
            // =====================================
            modelBuilder.Entity<UserPermission>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserPermissions)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // =====================================
            // UserPermission -> Feature
            // 1 Feature has many UserPermissions
            // =====================================
            modelBuilder.Entity<UserPermission>()
                .HasOne(x => x.Feature)
                .WithMany(x => x.UserPermissions)
                .HasForeignKey(x => x.FeatureID)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================
            // UserPermission -> Permission
            // 1 Permission has many UserPermissions
            // =====================================
            modelBuilder.Entity<UserPermission>()
                .HasOne(x => x.Permission)
                .WithMany(x => x.UserPermissions)
                .HasForeignKey(x => x.PermissionID)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================================
            // RefreshToken -> User
            // 1 User has many RefreshTokens
            // =====================================


            // =====================================
            // Unique Constraint
            // Username must be unique
            // =====================================
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();

            // =====================================
            // Prevent duplicate permission
            // Example:
            // User A cannot have
            // ROOM + CREATE twice
            // =====================================
            modelBuilder.Entity<UserPermission>()
                .HasIndex(x => new
                {
                    x.UserID,
                    x.FeatureID,
                    x.PermissionID
                })
                .IsUnique();
        }
    }
}