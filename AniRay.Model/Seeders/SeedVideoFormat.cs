using AniRay.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Seeders
{
    public class SeedVideoFormat : IEntityTypeConfiguration<VideoFormat>
    {
        public void Configure (EntityTypeBuilder<VideoFormat> builder)
        {
            builder.HasData
                (
                    new VideoFormat()
                    {
                        Id = 1,
                        Name = "720p",
                        IsDeleted = false
                    },
                    new VideoFormat()
                    {
                        Id = 2,
                        Name = "1080p",
                        IsDeleted = false
                    },
                    new VideoFormat()
                    {
                        Id = 3,
                        Name = "1080i",
                        IsDeleted = false
                    },
                    new VideoFormat()
                    {
                        Id = 4,
                        Name = "2160p (4k)",
                        IsDeleted = false
                    }
                );
        }
    }
}
