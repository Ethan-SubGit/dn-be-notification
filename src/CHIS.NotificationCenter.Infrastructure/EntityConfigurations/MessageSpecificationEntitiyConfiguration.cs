using System;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class MessageSpecificationEntitiyConfiguration : IEntityTypeConfiguration<MessageSpecification>
    {
        public void Configure(EntityTypeBuilder<MessageSpecification> builder)
        {
            if (builder != null)
            {
                var traceConverter = new ObjectToJsonConverter<Trace>();

                builder.ToTable($"{NotificationCenterContext.DOMAIN_NAME}_{nameof(MessageSpecification)}");
                builder.HasKey(x => x.Id);
                //builder.HasKey(x => new { x.Id, x.HospitalId, x.TenantId });
                builder.Property(x => x.Id).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired();
                builder.Property(x => x.TenantId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.HospitalId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.ServiceType).HasColumnTypeForCHIS(NormalDataType.INT).IsRequired(true);
                builder.Property(x => x.ServiceCode).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.MessageCategory).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.Classification).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.Description).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 700).IsRequired(false);
                builder.Property(x => x.PredefinedContent).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 700).IsRequired(false);
                builder.Property(x => x.PostActionType).HasColumnTypeForCHIS(NormalDataType.INT).IsRequired(true);
                builder.Property(x => x.IsSelectPatientByActiveEncounter).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true).HasDefaultValue(false);
                builder.Property(x => x.IsForceToSendInboxSmsMessage).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true).HasDefaultValue(false);

                builder.Property(x => x.IsSystemProperty).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true);
                builder.Property(x => x.IsDeleted).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true).HasDefaultValue(false);
                builder.Property(x => x.IsAddRecipient).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true).HasDefaultValue(false);

                builder.Property(x => x.MessageCallbackNoConfigId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(p => p.DataFirstRegisteredDateTimeUtc)
             .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.DataLastModifiedDateTimeUtc)
                    .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.Trace).HasConversion(traceConverter)
                                .HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.HasIndex(x => x.ServiceCode);
                builder.HasIndex(x => x.ServiceType);
                builder.HasIndex(x => x.MessageCategory);
                builder.HasIndex(x => new { x.HospitalId, x.TenantId });
                builder.HasIndex(x => x.IsDeleted);
                builder.HasIndex(x => x.MessageCallbackNoConfigId);
            }
        }
    }
}
