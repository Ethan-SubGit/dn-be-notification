using System;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class SmsReceiveLogEntityConfiguration : IEntityTypeConfiguration<SmsReceiveLog>
    {
        public void Configure(EntityTypeBuilder<SmsReceiveLog> builder)
        {
            if (builder != null)
            {
                var traceConverter = new ObjectToJsonConverter<Trace>();
                var mergingConverter = new ObjectToJsonConverter<List<MergingPatientGround>>();

                string TablName = $"{NotificationCenterContext.DOMAIN_NAME}_{nameof(SmsReceiveLog)}";
                builder.ToTable(TablName);
                //builder.ToTable(nameof(SmsMessageSentLog), NotificationCenterContext.DEFAULT_SCHEMA);
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasMaxLength(100).IsRequired();
                builder.Property(x => x.TenantId                        ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.HospitalId                      ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.SmsRecipientType).HasColumnTypeForCHIS(NormalDataType.INT).IsRequired(true );
                builder.Property(x => x.Name                            ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 200).IsRequired(false);
                builder.Property(x => x.Mobile                          ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 50).IsRequired(false);
                builder.Property(x => x.Content                         ).HasColumnTypeForCHIS(NormalDataType.TEXT).IsRequired(false);
                builder.Property(x => x.IsSuccess                       ).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true );
                builder.Property(x => x.SentTimeStamp                   ).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(false );
                builder.Property(x => x.MessageDispatchItemId           ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true ).HasDefaultValue("");
                builder.Property(x => x.ActorId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.IsAgreeToUsePrivacyData).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true);
                builder.Property(x => x.IsBlocked).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true);

                //add column (from TA API)
                builder.Property(x => x.SmsId                           ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false).HasDefaultValue("");
                builder.Property(x => x.MessageId                       ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false).HasDefaultValue("");
                builder.Property(x => x.CountryCode                     ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 10).IsRequired(false).HasDefaultValue("");
                builder.Property(x => x.CompleteTime                    ).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(false).HasDefaultValue(null);
                builder.Property(x => x.ContentType                     ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 50).IsRequired(false).HasDefaultValue("");
                builder.Property(x => x.telcoCode                       ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 50).IsRequired(false).HasDefaultValue("");
                builder.Property(x => x.StatusMessage                   ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 200).IsRequired(false).HasDefaultValue("");
                builder.Property(x => x.StatusName                      ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 200).IsRequired(false).HasDefaultValue("");
                builder.Property(x => x.StatusCode                      ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 200).IsRequired(false).HasDefaultValue("");
                builder.Property(x => x.RequestTime                     ).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(false).HasDefaultValue(null);

                builder.Property(x => x.SentTimeStampUtcPack)
                         .HasConversion(new ObjectToJsonConverter<UtcPack>())
                         .HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.Property(p => p.MergingPatientGrounds).HasConversion(mergingConverter)
                            .HasColumnTypeForCHIS(NormalDataType.TEXT).IsRequired(false);

                builder.Property(x => x.PatientContactRelationShipCode).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.PatientContactClassificationCode).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);

                builder.Property(p => p.DataFirstRegisteredDateTimeUtc).HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.DataLastModifiedDateTimeUtc)
                    .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.Trace).HasConversion(traceConverter)
                                .HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.HasIndex(p => p.IsBlocked);
                builder.HasIndex(p => p.MessageDispatchItemId);
                builder.HasIndex(p => p.MessageId);
                builder.HasIndex(p => new { p.HospitalId, p.TenantId });
                builder.HasIndex(p => p.ActorId);
                builder.HasIndex(p => p.SmsRecipientType);
                //builder.OwnsOne(x => x.SentTimeStampUtcPack).Property(p => p.TimeZoneId ).HasColumnName("SentTimeStampTimeZoneId").HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                //builder.OwnsOne(x => x.SentTimeStampUtcPack).Property(p => p.DateTimeUtc).HasColumnName("SentTimeStampUtc"       ).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(true);

                //builder.HasOne<MessageDispatchItem>().WithMany().HasForeignKey(i => i.MessageDispatchItemId).HasConstraintName($"FK_{TablName}_MDI_Id"); ;
            }
        }
    }
}
