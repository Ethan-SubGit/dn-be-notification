using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.CommonModels
{
    public class SmsReceiveLogDto
    {

        public string Id { get; set; }
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public string Mobile { get; set; }
        public string MobileWithNoMasking { get; set; }

        public string Content { get; set; }
        public string ContentType { get; set; }
        public string CountryCode { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsAgreeToUsePrivacyData { get; set; }
        public string Name { get; set; }
        public DateTime? RequestTime { get; set; }
        public DateTime? CompleteTime { get; set; }
        public DateTime? SentTimeStamp { get; set; }
        public string SmsId { get; set; }
        public SmsRecipientType SmsRecipientType { get; set; }
        public string MessageDispatchItemId { get; set; }
        public string MessageId { get; set; }
        public string StatusCode { get; set; }
        public string StatusName { get; set; }
        public string StatusMessage { get; set; }
        public string telcoCode { get; set; }
        public string PatientContactClassificationCode { get; set; }
        public string PatientContactRelationShipCode { get; set; }
        public UtcPack SentTimeStampUtcPack { get; set; }
        /// <summary>
        /// 수신자ID
        /// </summary>
        public string ActorId { get; set; }
        /// <summary>
        /// 발송자 ID
        /// </summary>
        public string SenderId { get; set; }
    }
}
