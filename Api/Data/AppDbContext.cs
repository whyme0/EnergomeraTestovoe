using Api.Models;
using Api.Topology;
using Api.Topology.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Field> Fields { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Coordinate> Coordinates { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Field>()
                .HasOne(f => f.Locations)
                .WithOne()
                .HasForeignKey<Location>(l => l.FieldId);
            modelBuilder.Entity<Field>()
                .HasIndex(f => f.Name).IsUnique();

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Center)
                .WithMany()
                .HasForeignKey(l => l.CenterId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Location>()
                .HasMany(l => l.Polygon)
                .WithOne(c => c.Location)
                .HasForeignKey(c => c.LocationId);
        }
    }
}
