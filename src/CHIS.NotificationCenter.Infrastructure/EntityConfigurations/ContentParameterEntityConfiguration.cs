using System;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class ContentParameterEntityConfiguration : IEntityTypeConfiguration<ContentParameter>
    {
        public void Configure(EntityTypeBuilder<ContentParameter> builder)
        {
            if (builder != null)
            {
                var traceConverter = new ObjectToJsonConverter<Trace>();

                builder.ToTable($"{NotificationCenterContext.DOMAIN_NAME}_{nameof(ContentParameter)}");
                builder.HasKey(x => x.Id);
                builder.Property(p => p.Id).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired();
                builder.Property(x => x.ParameterValue).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.MessageDispatchItemId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.TenantId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.HospitalId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(p => p.DataFirstRegisteredDateTimeUtc)
             .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.DataLastModifiedDateTimeUtc)
                    .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.Trace).HasConversion(traceConverter)
                                .HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.HasIndex(x => x.MessageDispatchItemId);
                builder.HasIndex(x => new { x.HospitalId, x.TenantId });

            }
        }
    }
}
