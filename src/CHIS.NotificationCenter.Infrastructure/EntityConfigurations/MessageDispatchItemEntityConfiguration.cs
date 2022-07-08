using System;
using System.Collections.Generic;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CHIS.Framework.Core.Extension;
using CHIS.NotificationCenter.Infrastructure.Utils;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Infrastructure.EntityConfigurations
{
    public class MessageDispatchItemEntityConfiguration : IEntityTypeConfiguration<MessageDispatchItem>
    {
        public void Configure(EntityTypeBuilder<MessageDispatchItem> builder)
        {
            if (builder != null)
            {
                var mergingConverter = new ObjectToJsonConverter<List<MergingPatientGround>>();
                var traceConverter = new ObjectToJsonConverter<Trace>();

                builder.ToTable($"{NotificationCenterContext.DOMAIN_NAME}_{nameof(MessageDispatchItem)}");
                //builder.ToTable(nameof(MessageDispatchItem), NotificationCenterContext.DEFAULT_SCHEMA);
                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id).HasMaxLength(100).IsRequired();
                builder.Property(x => x.TenantId                         ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.HospitalId                       ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.ServiceType                      ).HasColumnTypeForCHIS(NormalDataType.INT).IsRequired(true);
                builder.Property(x => x.ServiceCode                      ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(true);
                builder.Property(x => x.MessagePriority                  ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 10).IsRequired(false);
                builder.Property(x => x.SenderId                         ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.SentTimeStamp                    ).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(false);
                builder.Property(x => x.IsUsingPredefinedContent         ).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true);
               
                builder.Property(x => x.Title                            ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 200).IsRequired(false);
                builder.Property(x => x.Content).HasColumnTypeForCHIS(NormalDataType.TEXT).IsRequired(false);
                builder.Property(x => x.SmsContentByInbox                ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 700).IsRequired(false);
                builder.Property(x => x.IsDispatched                     ).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true);
                builder.Property(x => x.PatientId                        ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.EncounterId                      ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.IntegrationType                  ).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);
                builder.Property(x => x.IntegrationAddress               ).HasColumnTypeForCHIS(NormalDataType.TEXT).IsRequired(false);
                builder.Property(x => x.IntegrationParameter             ).HasColumnTypeForCHIS(NormalDataType.TEXT).IsRequired(false);
                builder.Property(x => x.IsReservedSms                    ).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true );
                builder.Property(x => x.ReservedSmsDateTime              ).HasColumnTypeForCHIS(NormalDataType.DATETIME).IsRequired(false);

                builder.Property(x => x.IsCanceled).HasColumnTypeForCHIS(NormalDataType.BIT).IsRequired(true).HasDefaultValue(false);
                builder.Property(x => x.ReferenceId).HasColumnTypeForCHIS(LengthDataType.VARCHAR, 100).IsRequired(false);

                builder.Property(x => x.SentTimeStampUtcPack)
                     .HasConversion(new ObjectToJsonConverter<UtcPack>())
                     .HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.Property(x => x.ReservedSmsDateTimeUtcPack)
                     .HasConversion(new ObjectToJsonConverter<UtcPack>())
                     .HasColumnTypeForCHIS(NormalDataType.TEXT);
                //builder.Property(x => x.ReservedSmsDateTimeUtcPack)
                //     .HasConversion(new ObjectToJsonConverter<UtcPack>())
                ////     .HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.Property(p => p.MergingPatientGroundsList).HasConversion(mergingConverter)
                                   .HasColumnTypeForCHIS(NormalDataType.TEXT).IsRequired(false);

                builder.Property(x => x.CommunicationNoteMessageDeliveryOption).HasColumnTypeForCHIS(NormalDataType.INT).IsRequired(true);

                builder.Property(p => p.DataFirstRegisteredDateTimeUtc)
             .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.DataLastModifiedDateTimeUtc)
                    .HasColumnTypeForCHIS(NormalDataType.DATETIME);

                builder.Property(p => p.Trace).HasConversion(traceConverter)
                                .HasColumnTypeForCHIS(NormalDataType.TEXT);

                //builder.Property<List<MergingPatientGround>>("MergingPatientGround").HasField("_mergingPatientGroundList")
                //.HasConversion(mergingConverter).HasColumnTypeForCHIS(NormalDataType.TEXT);

                builder.HasIndex(x => x.SenderId);
                builder.HasIndex(x => x.PatientId);
                builder.HasIndex(x => x.IsCanceled);
                builder.HasIndex(x => x.ServiceCode);
                builder.HasIndex(x => new { x.HospitalId, x.TenantId });
            }
        }
    }
}
