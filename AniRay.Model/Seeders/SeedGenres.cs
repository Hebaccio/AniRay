using AniRay.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Seeders
{
    public class SeedGenres : IEntityTypeConfiguration<Genre>
    {
        public void Configure (EntityTypeBuilder<Genre> builder)
        {
            builder.HasData (
                new Genre()
                {
                    Id = 1,
                    Name = "Action",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 2,
                    Name = "Adventure",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 3,
                    Name = "Comedy",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 4,
                    Name = "Drama",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 5,
                    Name = "Ecchi",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 6,
                    Name = "Fantasy",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 7,
                    Name = "Horror",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 8,
                    Name = "Mahou Shojo",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 9,
                    Name = "Mecha",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 10,
                    Name = "Music",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 11,
                    Name = "Mystery",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 12,
                    Name = "Psychological",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 13,
                    Name = "Romance",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 14,
                    Name = "Sci-Fi",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 15,
                    Name = "Slice Of Life",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 16,
                    Name = "Sports",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 17,
                    Name = "Supernatural",
                    IsDeleted = false
                },
                new Genre()
                {
                    Id = 18,
                    Name = "Thriller",
                    IsDeleted = false
                }
             );
        }
    }
}
