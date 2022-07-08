using System;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class RecipientPolicyProtocolEntityConfiguration : IEntityTypeConfiguration<RecipientPolicyProtocol>
    {
        public void Configure(EntityTypeBuilder<RecipientPolicyProtocol> builder)
        {
            if (builder != null)
            {
                var traceConverter = new ObjectToJsonConverter<Trace>();

                builder.ToTable($"{NotificationCenterContext.DOMAIN_NAME}_{nameof(RecipientPolicyProtocol)}");
                builder.Property(p => p.PolicyCode          ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired();
                builder.Property(p => p.Type                ).HasColumnTypeForCHIS(NormalDataType.INT).IsRequired();
                builder.Property(p => p.Name                ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 200).IsRequired(false);
                builder.Property(p => p.Description         ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 700).IsRequired(false);
                //builder.Property(x => x.ResolvingGroupCode  ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.TenantId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);

                builder.Property(p => p.DataFirstRegisteredDateTimeUtc)
             .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.DataLastModifiedDateTimeUtc)
                    .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.Trace).HasConversion(traceConverter)
                                .HasColumnTypeForCHIS(NormalDataType.TEXT);

                //builder.Property(x => x.HospitalId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);

                builder.HasKey(x => x.PolicyCode);
                builder.HasIndex(x => x.Type);
                builder.HasIndex(x => x.PolicyCode);
            }
        }
    }
}
