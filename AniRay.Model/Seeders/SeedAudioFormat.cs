using AniRay.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Seeders
{
    public class SeedAudioFormat : IEntityTypeConfiguration<AudioFormat>
    {
        public void Configure (EntityTypeBuilder<AudioFormat> builder)
        {
            builder.HasData
                (
                    new AudioFormat()
                    {
                        Id = 1,
                        Name = "Dolby TrueHD",
                        IsDeleted = false
                    },
                    new AudioFormat()
                    {
                        Id = 2,
                        Name = "DTS-HD Master Audio",
                        IsDeleted = false
                    },
                    new AudioFormat()
                    {
                        Id = 3,
                        Name = "Dolby Digital",
                        IsDeleted = false
                    },
                    new AudioFormat()
                    {
                        Id = 4,
                        Name = "DTS",
                        IsDeleted = false
                    },
                    new AudioFormat()
                    {
                        Id = 5,
                        Name = "LPCM",
                        IsDeleted = false
                    },
                    new AudioFormat()
                    {
                        Id = 6,
                        Name = "Dolby Atmos",
                        IsDeleted = false
                    },
                    new AudioFormat()
                    {
                        Id = 7,
                        Name = "DTS:X",
                        IsDeleted = false
                    },
                    new AudioFormat()
                    {
                        Id = 8,
                        Name = "Mono",
                        IsDeleted = false
                    },
                    new AudioFormat()
                    {
                        Id = 9,
                        Name = "Stereo",
                        IsDeleted = false
                    }
                );
        }
    }
}
