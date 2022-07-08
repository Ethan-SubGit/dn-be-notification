using System;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class SmsSendLogEntityConfiguration : IEntityTypeConfiguration<SmsSendLog>
    {
        public void Configure(EntityTypeBuilder<SmsSendLog> builder)
        {
            if (builder != null)
            {
                var traceConverter = new ObjectToJsonConverter<Trace>();
                string TablName = $"{NotificationCenterContext.DOMAIN_NAME}_{nameof(SmsSendLog)}";
                builder.ToTable(TablName);
                //builder.ToTable(nameof(SmsMessageSentLog), NotificationCenterContext.DEFAULT_SCHEMA);
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired();
                builder.Property(x => x.TenantId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.HospitalId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                
                builder.Property(x => x.Content).HasColumnTypeForCHIS(NormalDataType.TEXT).IsRequired(false);
                builder.Property(x => x.CallingNumber).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 50).IsRequired(false);

                builder.Property(x => x.IsReservedSms).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true);
                builder.Property(x => x.ReservedTime).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(false);
                builder.Property(x => x.ReservedTimeUtcPack)
                                         .HasConversion(new ObjectToJsonConverter<UtcPack>())
                                         .HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.Property(x => x.ExecutionTime).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(false);
                builder.Property(x => x.ExecutionTimeUtcPack)
                                         .HasConversion(new ObjectToJsonConverter<UtcPack>())
                                         .HasColumnTypeForCHIS(NormalDataType.TEXT);
                builder.Property(x => x.SmsProgressStatus).HasColumnTypeForCHIS(NormalDataType.INT).IsRequired(true);
                builder.Property(x => x.SmsRecipientType).HasColumnTypeForCHIS(NormalDataType.INT).IsRequired(true);

                builder.Property(x => x.MessageDispatchItemId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true).HasDefaultValue("");
               
                builder.Property(x => x.SmsTraceId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.CallStatusCode).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.ErrorMessage).HasColumnTypeForCHIS(NormalDataType.TEXT).IsRequired(false);
                builder.Property(x => x.SenderId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.ServiceCode).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);

                builder.Property(p => p.DataFirstRegisteredDateTimeUtc)
            .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.DataLastModifiedDateTimeUtc)
                    .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.Trace).HasConversion(traceConverter)
                                .HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.HasIndex(p => p.MessageDispatchItemId);
                builder.HasIndex(p => p.SmsRecipientType);
                builder.HasIndex(p => p.SmsProgressStatus);
                //builder.HasIndex(p => new { p.HospitalId, p.TenantId });
                builder.HasIndex(p => p.HospitalId);
                builder.HasIndex(p => p.TenantId);
                builder.HasIndex(p => p.SenderId);
                //builder.OwnsOne(x => x.SentTimeStampUtcPack).Property(p => p.TimeZoneId).HasColumnName("SentTimeStampTimeZoneId").HasColumnType("nvarchar(100)").IsRequired(true);
                //builder.OwnsOne(x => x.SentTimeStampUtcPack).Property(p => p.DateTimeUtc).HasColumnName("SentTimeStampUtc").HasColumnType("DateTime").IsRequired(true);

                //builder.HasOne<MessageDispatchItem>().WithMany().HasForeignKey(i => i.MessageDispatchItemId).HasConstraintName($"FK_{TablName}_MDI_Id"); ;



            }
        }
    }
}
