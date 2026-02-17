using AniRay.Model.Entities;
using AniRay.Model.Seeders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace AniRay.Model.Data
{
    public class AniRayDbContext : DbContext
    {
        public AniRayDbContext(DbContextOptions<AniRayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserStatus> UserStatuses { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<BluRay> BluRays { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<OrderBluRay> OrderBluRays { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestUser> RequestUsers { get; set; }
        public DbSet<UserFavorites> UserFavorites { get; set; }
        public DbSet<BluRayCart> BluRayCarts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserCart> UserCarts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BluRay>()
                .Property(b => b.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.FullPrice)
                .HasPrecision(18, 2);

            AddDataSeeders(modelBuilder);
        }

        private void AddDataSeeders(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SeedAudioFormat());
            modelBuilder.ApplyConfiguration(new SeedGender());
            modelBuilder.ApplyConfiguration(new SeedGenres());
            modelBuilder.ApplyConfiguration(new SeedOrderStatus());
            modelBuilder.ApplyConfiguration(new SeedUserRoles());
            modelBuilder.ApplyConfiguration(new SeedUserStatus());
            modelBuilder.ApplyConfiguration(new SeedVideoFormat());
        }
    }
}
