using System;
using CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class EmployeeMessageInstanceEntityConfiguration : IEntityTypeConfiguration<EmployeeMessageInstance>
    {
        public void Configure(EntityTypeBuilder<EmployeeMessageInstance> builder)
        {
            if (builder != null)
            {
                var traceConverter = new ObjectToJsonConverter<Trace>();

                string TablName = $"{NotificationCenterContext.DOMAIN_NAME}_{nameof(EmployeeMessageInstance)}";
                builder.ToTable(TablName);
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired();
                builder.Property(x => x.IsInbound).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true );
                builder.Property(x => x.IsHandled).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true );
                builder.Property(x => x.HandleTime).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(false);
                builder.Property(x => x.HandleTimeUtcPack)
                                         .HasConversion(new ObjectToJsonConverter<UtcPack>())
                                         .HasColumnTypeForCHIS(NormalDataType.TEXT);
                builder.Property(x => x.IsReaded).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true );
                builder.Property(x => x.ReadTime).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(false);
                builder.Property(x => x.ReadTimeUtcPack)
                                         .HasConversion(new ObjectToJsonConverter<UtcPack>())
                                         .HasColumnTypeForCHIS(NormalDataType.TEXT);
                builder.Property(x => x.Title).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 200).IsRequired();
                builder.Property(x => x.Content).HasColumnTypeForCHIS(NormalDataType.TEXT).IsRequired(false);
                builder.Property(x => x.MessageDispatchItemId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true );
                builder.Property(x => x.EmployeeMessageBoxId ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true ); //FK
                builder.HasOne<EmployeeMessageBox>().WithMany(f => f.EmployeeMessageInstances).HasForeignKey("EmployeeMessageBoxId").OnDelete(DeleteBehavior.Restrict);

                builder.Property(x => x.ServiceCode).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);

                builder.Property(x => x.SentTimeStamp).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(true);
                builder.Property(x => x.TenantId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.HospitalId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.EmployeeId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired();

                builder.Property(p => p.DataFirstRegisteredDateTimeUtc)
             .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.DataLastModifiedDateTimeUtc)
                    .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.Trace).HasConversion(traceConverter)
                                .HasColumnTypeForCHIS(NormalDataType.TEXT);

                //builder.HasOne<EmployeeMessageBox>().WithMany(m => m.EmployeeMessageInstances).HasForeignKey(i => i.EmployeeMessageBoxId).HasConstraintName($"FK_{TablName}_EMB_Id");
                builder.HasIndex(x => x.MessageDispatchItemId);
                builder.HasIndex(x => x.IsReaded);
                builder.HasIndex(x => x.IsHandled);
                builder.HasIndex(x => x.IsInbound);
                builder.HasIndex(x => x.EmployeeMessageBoxId);

                //builder.HasIndex(x => new { x.EmployeeId, x.SentTimeStamp });
                builder.HasIndex(x => x.EmployeeId);
                builder.HasIndex(x => new { x.HospitalId, x.TenantId });
                
            }
        }
    }
}
