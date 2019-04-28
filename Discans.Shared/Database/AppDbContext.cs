using Discans.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Discans.Shared.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : 
            base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => 
            base.OnModelCreating(modelBuilder);
        
        public DbSet<Manga> Mangas { get; set; }
        public DbSet<ServerChannel> ServerChannels { get; set; }
        public DbSet<UserAlert> UserAlerts { get; set; }
        public DbSet<ServerAlert> ServerAlerts { get; set; }
        public DbSet<PrivateAlert> PrivateAlerts { get; set; }
        public DbSet<ServerLocalizer> ServerLocalizer { get; set; }
        public DbSet<UserLocalizer> UserLocalizer { get; set; }
    }
}
