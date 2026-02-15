using AniRay.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Seeders
{
    public class SeedUserRoles : IEntityTypeConfiguration<UserRole>
    {
        public void Configure (EntityTypeBuilder<UserRole> builder)
        {
            builder.HasData
                (
                    new UserRole()
                    {
                        Id = 1,
                        Name = "User",
                        IsDeleted = false
                    },
                    new UserRole()
                    {
                        Id = 2,
                        Name = "Employee",
                        IsDeleted = false
                    },
                    new UserRole()
                    {
                        Id = 3,
                        Name = "Boss",
                        IsDeleted = false
                    }
                );
        }
    }
}
