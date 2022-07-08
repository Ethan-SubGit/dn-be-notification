using System;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class DepartmentPolicyEntityConfiguration : IEntityTypeConfiguration<DepartmentPolicy>
    {
        public void Configure(EntityTypeBuilder<DepartmentPolicy> builder)
        {
            if (builder != null)
            {
                var traceConverter = new ObjectToJsonConverter<Trace>();

                builder.ToTable($"{NotificationCenterContext.DOMAIN_NAME}_{nameof(DepartmentPolicy)}");
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasMaxLength(100).IsRequired();
                builder.Property(x => x.ProtocolCode).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.DepartmentId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.OccupationId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.JobPositionId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.WorkPlaceId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.MessageSpecificationId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true); //FK
                //builder.HasOne<MessageSpecification>().WithMany(f => f.DepartmentPolicies).HasForeignKey("MessageSpecificationId").OnDelete(DeleteBehavior.Restrict);

                builder.Property(x => x.TenantId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.HospitalId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);

                builder.Property(p => p.DataFirstRegisteredDateTimeUtc)
             .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.DataLastModifiedDateTimeUtc)
                    .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.Trace).HasConversion(traceConverter)
                                .HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.HasIndex(x => x.MessageSpecificationId);
                builder.HasIndex(x => new { x.HospitalId, x.TenantId });
            }
        }
    }
}
