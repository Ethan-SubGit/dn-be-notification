using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class MessageAttachmentEntiryConfiguration : IEntityTypeConfiguration<MessageAttachment>
    {
        public void Configure(EntityTypeBuilder<MessageAttachment> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            var traceConverter = new ObjectToJsonConverter<Trace>();

            builder.ToTable($"{NotificationCenterContext.DOMAIN_NAME}_{nameof(MessageAttachment)}");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);

            builder.Property(p => p.ContentType      ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
            builder.Property(p => p.Extension        ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
            builder.Property(p => p.FileKey              ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
            builder.Property(p => p.OriginalFileName ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 300).IsRequired(true);
            builder.Property(p => p.SavedFileName    ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 300).IsRequired(false);
            builder.Property(p => p.SavedFilePath    ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 300).IsRequired(false);
            builder.Property(p => p.FileSize).HasColumnTypeForCHIS(NormalDataType.INT).IsRequired(true).HasDefaultValue(0);
            builder.Property(p => p.Url              ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 300).IsRequired(true);
            builder.Property(p => p.MessageDispatchItemId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);

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
