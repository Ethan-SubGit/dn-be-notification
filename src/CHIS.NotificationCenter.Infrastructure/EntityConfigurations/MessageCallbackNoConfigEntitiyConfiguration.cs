using System;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class MessageCallbackNoConfigEntitiyConfiguration : IEntityTypeConfiguration<MessageCallbackNoConfig>
    {
        public void Configure(EntityTypeBuilder<MessageCallbackNoConfig> builder)
        {
            if (builder != null)
            {
                var traceConverter = new ObjectToJsonConverter<Trace>();

                builder.ToTable($"{NotificationCenterContext.DOMAIN_NAME}_{nameof(MessageCallbackNoConfig)}");


                builder.HasKey(k => k.Id);
                builder.Property(p => p.Id).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                //builder.Property(p => p.MessageSpecificationId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                //builder.HasOne<MessageSpecification>().WithMany(f => f.MessageTemplates).HasForeignKey("MessageSpecificationId").OnDelete(DeleteBehavior.Restrict);


                builder.Property(p => p.CallbackTitle).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.Description).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 700).IsRequired(false);
                builder.Property(p => p.CallbackNo).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(p => p.IsDeleted).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true).HasDefaultValue(false);
                builder.Property(p => p.IsSystemProperty).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true).HasDefaultValue(false);
                builder.Property(p => p.IsMaster).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true).HasDefaultValue(false);

                builder.Property(p => p.TenantId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(p => p.HospitalId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);


                builder.Property(p => p.Trace).HasConversion(traceConverter).HasColumnTypeForCHIS(NormalDataType.TEXT);
                builder.Property(p => p.DataFirstRegisteredDateTimeUtc).HasColumnTypeForCHIS(NormalDataType.DATETIME);
                builder.Property(p => p.DataLastModifiedDateTimeUtc).HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.HasIndex(p => p.IsDeleted);
                builder.HasIndex(p => p.IsMaster);
                builder.HasIndex(x => new { x.HospitalId, x.TenantId });
            }
        }
    }
}
