using AniRay.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Seeders
{
    public class SeedUserStatus : IEntityTypeConfiguration<UserStatus>
    {
        public void Configure (EntityTypeBuilder<UserStatus> builder)
        {
            builder.HasData
                (
                    new UserStatus()
                    {
                        Id = 1,
                        Name = "Active",
                        IsDeleted = false,
                        StatusForUser = true,
                        StatusForEmployee = true,
                    },
                    new UserStatus()
                    {
                        Id = 2,
                        Name = "Suspended",
                        IsDeleted = false,
                        StatusForUser = true,
                        StatusForEmployee = false,
                    },
                    new UserStatus()
                    {
                        Id = 3,
                        Name = "Deleted",
                        IsDeleted = false,
                        StatusForUser = true,
                        StatusForEmployee = false
                    },
                    new UserStatus()
                    {
                        Id = 4,
                        Name = "Fired Or Quit",
                        IsDeleted = false,
                        StatusForUser = false,
                        StatusForEmployee = true
                    }
                );
        }
    }
}
