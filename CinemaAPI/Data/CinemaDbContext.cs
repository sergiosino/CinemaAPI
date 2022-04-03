using CinemaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaAPI.Data
{
    public class CinemaDbContext : DbContext
    {
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                .HasOne<Movie>(s => s.Movie)
                .WithMany(g => g.Reservations)
                .HasForeignKey(s => s.MovieId);
            modelBuilder.Entity<Reservation>()
                .HasOne<User>(s => s.User)
                .WithMany(g => g.Reservations)
                .HasForeignKey(s => s.UserId);
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
