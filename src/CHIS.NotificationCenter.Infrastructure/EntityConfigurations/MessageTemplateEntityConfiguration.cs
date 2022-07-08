using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class MessageTemplateEntityConfiguration : IEntityTypeConfiguration<MessageTemplate>
    {
        public void Configure(EntityTypeBuilder<MessageTemplate> builder)
        {
            if (builder == null)
            {
                throw new ArgumentException(nameof(builder));
            }

            var traceConverter = new ObjectToJsonConverter<Trace>();
            builder.ToTable($"{NotificationCenterContext.DOMAIN_NAME}_{nameof(MessageTemplate)}");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
            builder.Property(p => p.MessageSpecificationId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
            builder.HasOne<MessageSpecification>().WithMany(f => f.MessageTemplates).HasForeignKey("MessageSpecificationId").OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.ContentTemplateScope).HasColumnTypeForCHIS(NormalDataType.INT).IsRequired(true);
            builder.Property(p => p.TemplateTitle).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
            builder.Property(p => p.ContentTemplate).HasColumnTypeForCHIS(NormalDataType.TEXT).IsRequired(false);
            builder.Property(p => p.IsDeleted).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true);

            builder.Property(p => p.EmployeeId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
            builder.Property(p => p.TenantId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
            builder.Property(p => p.HospitalId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);

            
            builder.Property(p => p.Trace).HasConversion(traceConverter).HasColumnTypeForCHIS(NormalDataType.TEXT);
            builder.Property(p => p.DataFirstRegisteredDateTimeUtc).HasColumnTypeForCHIS(NormalDataType.DATETIME);
            builder.Property(p => p.DataLastModifiedDateTimeUtc).HasColumnTypeForCHIS(NormalDataType.DATETIME);

            builder.HasIndex(p => p.IsDeleted);
            builder.HasIndex(x => new { x.HospitalId, x.TenantId });
        }
    }
}
