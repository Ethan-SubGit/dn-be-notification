using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Queries.ReadModels.SmsMonitoring
{
    /// <summary>
    /// 사용여부 체크..
    /// </summary>
    public class SmsLogForStatisticsView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }

        public string Content { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime? SentTimeStamp { get; set; }
        public string MessageDispatchItemId { get; set; }
        public string HospitalId { get; set; }
        public string TenantId { get; set; }
        public DateTime? CompleteTime { get; set; }
        public string ContentType { get; set; }
        public string CountryCode { get; set; }
        public string MessageId { get; set; }
        public DateTime? RequestTime { get; set; }
        public string SmsId { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string StatusName { get; set; }
        public string TelcoCode { get; set; }
        public SmsRecipientType SmsRecipientType { get; set; }
        public string ActorId { get; set; }
        public bool IsAgreeToUserPrivacyData { get; set; }
        public UtcPack SentTimeStampUtcPack { get; set; }
        //public string MergingPatientGrounds { get; set; }
        public string PatientContactClassificationCode { get; set; }
        public string PatientContactRelationShipCode { get; set; }
        public string SenderId { get; set; }


    }
}
