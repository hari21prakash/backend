using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.FullName).IsRequired(false).HasMaxLength(200);
                entity.Property(u => u.CreatedAt).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Projects");
                entity.HasKey(p => p.Id);

                // Only ProjectName is required. Every other column is nullable
                // (IsRequired(false) is actually the EF Core default for a `string?`
                // property, but it is spelled out here to make the contract explicit).
                entity.Property(p => p.ProjectName).IsRequired().HasMaxLength(200);

                entity.Property(p => p.Description).IsRequired(false);
                entity.Property(p => p.ProjectLink).IsRequired(false);
                entity.Property(p => p.VideoUrl).IsRequired(false);
                entity.Property(p => p.ImageUrl).IsRequired(false);

                // Npgsql maps string[] to a native PostgreSQL text[] column.
                entity.Property(p => p.Technologies).IsRequired(false);

                entity.Property(p => p.GithubUrl).IsRequired(false);
                entity.Property(p => p.LiveDemoUrl).IsRequired(false);
                entity.Property(p => p.Status).IsRequired(false).HasMaxLength(50);
                entity.Property(p => p.StartDate).IsRequired(false);
                entity.Property(p => p.EndDate).IsRequired(false);
                entity.Property(p => p.Category).IsRequired(false).HasMaxLength(100);
                entity.Property(p => p.ClientName).IsRequired(false).HasMaxLength(200);
                entity.Property(p => p.Notes).IsRequired(false);

                entity.Property(p => p.CreatedAt).IsRequired();
                entity.Property(p => p.UpdatedAt).IsRequired();

                entity.HasIndex(p => p.ProjectName);
                entity.HasIndex(p => p.Status);
                entity.HasIndex(p => p.Category);
            });
        }
    }
}
