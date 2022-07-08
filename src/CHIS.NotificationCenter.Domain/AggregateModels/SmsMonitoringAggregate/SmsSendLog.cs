using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.SeedWork;
//using CHIS.Share.NotificationCenter.Enum;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;

namespace CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate
{
    public class SmsSendLog : EntityBase
    {
        #region property
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public string Content { get; set; }
        public string CallingNumber { get; set; }

        public SmsRecipientType SmsRecipientType { get; set; }

        public bool IsReservedSms { get; set; }
        public DateTime? ReservedTime { get; set; }

        public UtcPack ReservedTimeUtcPack { get; set; }
        public DateTime? ExecutionTime { get; set; }
        public UtcPack ExecutionTimeUtcPack { get; set; }
        public SmsProgressStatus SmsProgressStatus { get; set; }


        public string MessageDispatchItemId { get; set; }

        public string SmsTraceId { get; set; }
        public string CallStatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string SenderId { get; set; }
        public string ServiceCode { get; set; }
        //20191113 신규추가된 컬럼
        public DateTime DataFirstRegisteredDateTimeUtc { get; set; }
        public DateTime DataLastModifiedDateTimeUtc { get; set; }
        public Trace Trace { get; set; } 
        #endregion

        public SmsSendLog() { }

        public SmsSendLog(
              string tenantId
            , string hospitalId
            , string content
            , string callingNumber
            , bool isReservedSms
            , DateTime? reservedTime
            , DateTime? executionTime
            , SmsProgressStatus smsProgressStatus
            
            , string messageDispatchItemId
            , string smsTraceId
            , string callStatusCode
            , string errorMessage
            , string senderId
            , SmsRecipientType smsRecipientType
            , string serviceCode
            , DateTime dataFirstRegisteredDateTimeUtc
            , DateTime dataLastModifiedDateTimeUtc
            , Trace trace = null
           )
        {
            TenantId = tenantId;
            HospitalId = hospitalId;
            Content = content;
            CallingNumber = callingNumber;
            IsReservedSms = isReservedSms;
            ReservedTime = reservedTime;
            ExecutionTime = executionTime;
            SmsProgressStatus = smsProgressStatus;
            MessageDispatchItemId = messageDispatchItemId;
            SmsTraceId = smsTraceId;
            CallStatusCode = callStatusCode;

            ErrorMessage = errorMessage;
            SenderId = senderId;

            SmsRecipientType = smsRecipientType;
            ServiceCode = serviceCode;
            Trace = trace;
            DataFirstRegisteredDateTimeUtc = dataFirstRegisteredDateTimeUtc;
            DataLastModifiedDateTimeUtc = dataLastModifiedDateTimeUtc;
        }
    }
}
