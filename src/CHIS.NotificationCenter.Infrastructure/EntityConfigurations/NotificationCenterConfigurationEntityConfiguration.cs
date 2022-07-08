using CHIS.NotificationCenter.Domain.AggregateModels.NotificationCenterConfigurationAggregate;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using CHIS.Framework.Core.Extension;
namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{

    public class NotificationCenterConfigurationEntityConfiguration : IEntityTypeConfiguration<NotificationCenterConfiguration>
    {
        public void Configure(EntityTypeBuilder<NotificationCenterConfiguration> builder)
        {
            if (builder == null)
            {
                throw new ArgumentException(nameof(builder));
            }
            builder.ToTable($"{NotificationCenterContext.DOMAIN_NAME}_{nameof(NotificationCenterConfiguration)}");

            builder.HasKey(k => k.Id);
            builder.Property(x => x.Id).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired();
            builder.Property(x => x.TenantId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
            builder.Property(x => x.HospitalId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);

            builder.Property(x => x.ConfigurationKey).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
            builder.Property(x => x.ConfigurationValue).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);

        }
    }
}
