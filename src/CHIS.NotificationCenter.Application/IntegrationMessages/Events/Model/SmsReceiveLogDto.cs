using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model
{
    public class SmsReceiveLogDto
    {
        //private SmsReceiveLog log;
        #region 변수

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
        #endregion


        /// <summary>
        /// creator
        /// </summary>
        /// <param name="id">smsReceiveLog.id</param>
        /// <param name="name">수신자명</param>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        /// <param name="isSuccess"></param>
        /// <param name="sentTimeStamp"></param>
        /// <param name="messageDispatchItemId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="tenantId"></param>
        /// <param name="completeTime"></param>
        /// <param name="contentType"></param>
        /// <param name="countryCode"></param>
        /// <param name="messageId"></param>
        /// <param name="requestTime"></param>
        /// <param name="smsId"></param>
        /// <param name="statusCode"></param>
        /// <param name="statusMessage"></param>
        /// <param name="statusName"></param>
        /// <param name="telcoCode"></param>
        /// <param name="smsRecipientType"></param>
        /// <param name="actorId"></param>
        /// <param name="isAgreeToUserPrivacyData"></param>
        /// <param name="sentTimeStampUtcPack"></param>
        /// <param name="patientContactClassificationCode"></param>
        /// <param name="patientContactRelationShipCode"></param>
        public SmsReceiveLogDto(
            string id, string name, string mobile, string content, bool isSuccess,
            DateTime? sentTimeStamp, string messageDispatchItemId, string hospitalId,
            string tenantId, DateTime? completeTime, string contentType,
            string countryCode, string messageId, DateTime? requestTime,
            string smsId, string statusCode, string statusMessage, string statusName,
            string telcoCode, SmsRecipientType smsRecipientType, string actorId,
            bool isAgreeToUserPrivacyData, UtcPack sentTimeStampUtcPack,
            string patientContactClassificationCode, string patientContactRelationShipCode,
            string senderId)
        {
            Id = id;
            Name = name;
            Mobile = mobile;
            Content = content;
            IsSuccess = isSuccess;
            SentTimeStamp = sentTimeStamp;
            MessageDispatchItemId = messageDispatchItemId;
            HospitalId = hospitalId;
            TenantId = tenantId;
            CompleteTime = completeTime;
            ContentType = contentType;
            CountryCode = countryCode;
            MessageId = messageId;
            RequestTime = requestTime;
            SmsId = smsId;
            StatusCode = statusCode;
            StatusMessage = statusMessage;
            StatusName = statusName;
            TelcoCode = telcoCode;
            SmsRecipientType = smsRecipientType;
            ActorId = actorId;
            IsAgreeToUserPrivacyData = isAgreeToUserPrivacyData;
            SentTimeStampUtcPack = sentTimeStampUtcPack;
            PatientContactClassificationCode = patientContactClassificationCode;
            PatientContactRelationShipCode = patientContactRelationShipCode;
            SenderId = senderId;
        }

        //public SmsReceiveLogDto(SmsReceiveLog log)
        //{
        //    this.log = log;
        //}
    }
}
