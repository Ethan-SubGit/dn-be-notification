﻿using System;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class EmployeeRecipientEntityConfiguration : IEntityTypeConfiguration<EmployeeRecipient>
    {
        public void Configure(EntityTypeBuilder<EmployeeRecipient> builder)
        {
            if (builder != null)
            {
                var traceConverter = new ObjectToJsonConverter<Trace>();

                builder.ToTable($"{NotificationCenterContext.DOMAIN_NAME}_{nameof(EmployeeRecipient)}");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired();

                builder.Property(x => x.EmployeeId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.MessageSpecificationId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);//FK
                //builder.HasOne<MessageSpecification>().WithMany(f => f.EmployeeRecipients).HasForeignKey("MessageSpecificationId").OnDelete(DeleteBehavior.Restrict);

                builder.Property(p => p.DataFirstRegisteredDateTimeUtc)
            .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.DataLastModifiedDateTimeUtc)
                    .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.Trace).HasConversion(traceConverter)
                                .HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.Property(x => x.TenantId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.HospitalId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);

                builder.HasIndex(x => x.MessageSpecificationId);
                builder.HasIndex(x => new { x.HospitalId, x.TenantId });

            }
        }
    }
}
