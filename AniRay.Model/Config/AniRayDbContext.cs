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
        public DbSet<UserFavorites> UserFavorites { get; set; }
        public DbSet<BluRayCart> BluRayCarts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserCart> UserCarts { get; set; }
        public DbSet<TwoWayAuth> twoWayAuths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureCompositeKeys(modelBuilder);
            ConfigureDecimalPrecision(modelBuilder);
            ConfigureIndexes(modelBuilder);
            AddDataSeeders(modelBuilder);
        }


        private void ConfigureCompositeKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BluRayCart>()
                .HasKey(bc => new { bc.UserCartId, bc.BluRayId });

            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            modelBuilder.Entity<OrderBluRay>()
                .HasKey(obr => new { obr.OrderId, obr.BluRayId });

            modelBuilder.Entity<UserFavorites>()
                .HasKey(uf => new { uf.UserId, uf.MovieId });
        }
        private void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BluRay>()
                .Property(b => b.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.FullPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<UserCart>()
                .Property(c => c.FullCartPrice)
                .HasPrecision(18, 2);
        }
        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            ConfigureBluRayIndexes(modelBuilder);
        }
        private void ConfigureBluRayIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BluRay>().HasIndex(b => b.MovieId);
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
