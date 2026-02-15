using AniRay.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Seeders
{
    public class SeedGender : IEntityTypeConfiguration<Gender>
    {
        public void Configure (EntityTypeBuilder<Gender> builder)
        {
            builder.HasData
                (
                    new Gender()
                    {
                        Id = 1,
                        Name = "Male",
                        IsDeleted = false
                    },
                    new Gender()
                    {
                        Id = 2,
                        Name = "Female",
                        IsDeleted = false
                    }
                );
        }
    }
}
