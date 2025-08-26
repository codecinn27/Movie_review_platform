using Microsoft.EntityFrameworkCore;
using MovieAPI.Models;

namespace MovieAPI.AppDataContext
{
    
    
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Review> Reviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.ToTable("Movies");
                entity.HasKey(x => x.Id);

               // Add unique constraint for Title
                entity.HasIndex(m => m.Title).IsUnique();
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Reviews");
                entity.HasKey(x => x.Id);

                // Ensure GUID is auto-generated
                entity.Property(x => x.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // 1 user -> many reviews
                entity.HasOne(r => r.User)          // Review has one User
                    .WithMany(u => u.Reviews)     // User has many Reviews
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // 1 movie -> many reviews (optional if you have Movie navigation)
                entity.HasOne(r => r.Movie)
                    .WithMany(m => m.Reviews)
                    .HasForeignKey(r => r.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);    

                // Optional: composite unique index to ensure one review per user per movie
                entity.HasIndex(r => new { r.UserId, r.MovieId })
                    .IsUnique();
            });

        }
    }
}