using Discans.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Discans.Shared.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Manga>(x =>
            //{
            //    x.HasKey(prop => prop.Id);
            //    x.Property(prop => prop.Id).ValueGeneratedNever();
            //    x.Property(prop => prop.Name).IsRequired();
            //    x.Property(prop => prop.LastRelease).IsRequired();

            //    x.HasMany(prop => prop.PrivateAlerts)
            //        .WithOne(prop => prop.Manga)
            //        .IsRequired()
            //        .OnDelete(DeleteBehavior.Cascade);

            //    x.HasMany(prop => prop.ServerAlerts)
            //        .WithOne(prop => prop.Manga)
            //        .IsRequired()
            //        .OnDelete(DeleteBehavior.Cascade);

            //    x.HasMany(prop => prop.UserAlerts)
            //        .WithOne(prop => prop.Manga)
            //        .IsRequired()
            //        .OnDelete(DeleteBehavior.Cascade);
            //});

            //modelBuilder.Entity<ServerAlert>(x =>
            //{
            //    x.HasKey(prop => prop.Id);
            //    x.Property(prop => prop.Id).ValueGeneratedOnAdd();
            //    x.Property(prop => prop.ServerId).IsRequired();
            //});

            //modelBuilder.Entity<UserAlert>(x =>
            //{
            //    x.HasKey(prop => prop.Id);
            //    x.Property(prop => prop.Id).ValueGeneratedOnAdd();
            //    x.Property(prop => prop.ServerId).IsRequired();
            //    x.Property(prop => prop.UserId).IsRequired();
            //});

            //modelBuilder.Entity<PrivateAlert>(x =>
            //{
            //    x.HasKey(prop => prop.Id);
            //    x.Property(prop => prop.Id).ValueGeneratedOnAdd();
            //    x.Property(prop => prop.UserId).IsRequired();
            //});

            //modelBuilder.Entity<ServerChannel>(x =>
            //{
            //    x.HasKey(prop => prop.Id);
            //    x.Property(prop => prop.Id).ValueGeneratedNever();
            //    x.Property(prop => prop.ChannelId).IsRequired();
            //});

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<Manga> Mangas { get; set; }
        public DbSet<ServerChannel> ServerChannels { get; set; }
        public DbSet<UserAlert> UserAlert { get; set; }
        public DbSet<ServerAlert> ServerAlert { get; set; }
        public DbSet<PrivateAlert> PrivateTrackings { get; set; }
    }
}
