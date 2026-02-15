using AniRay.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Seeders
{
    public class SeedOrderStatus : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure (EntityTypeBuilder<OrderStatus> builder)
        {
            builder.HasData
                (
                    new OrderStatus()
                    {
                        Id = 1,
                        Name = "In Progress",
                        IsDeleted = false
                    },
                    new OrderStatus()
                    {
                        Id = 2,
                        Name = "Cancelled",
                        IsDeleted = false
                    },
                    new OrderStatus()
                    {
                        Id = 3,
                        Name = "Rejected",
                        IsDeleted = false
                    },
                    new OrderStatus()
                    {
                        Id = 4,
                        Name = "Processed",
                        IsDeleted = false
                    }
                );
        }
    }
}
