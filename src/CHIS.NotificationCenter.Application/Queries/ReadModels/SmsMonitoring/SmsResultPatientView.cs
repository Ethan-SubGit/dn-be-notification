using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.Enum;
namespace CHIS.NotificationCenter.Application.Queries.ReadModels.SmsMonitoring
{
    public class SmsResultPatientView
    {
        #region property
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public string MessageDispatchItemId { get; set; }
        public string ServiceCode { get; set; }
        public string Classification { get; set; }
        public bool IsReservedSms { get; set; }
        public string Content { get; set; }
        public DateTime? ReservedTime { get; set; }
        public DateTime? ExecutionTime { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Mobile { get; set; }

        public string MobileWithNoMasking { get; set; }
        public SmsProgressStatus SmsProgressStatus { get; set; }
        public string SmsTraceId { get; set; }
        public string CallStatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string MessageId { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public string StatusName { get; set; }

        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string DisplayId { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public bool IsAgreeToUsePrivacyData { get; set; } 
        #endregion
    }
}
