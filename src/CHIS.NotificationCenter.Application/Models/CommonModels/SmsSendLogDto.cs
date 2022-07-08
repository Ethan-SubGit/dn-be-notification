using CHIS.NotificationCenter.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.CommonModels
{
    public class SmsSendLogDto
    {
        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public string Content { get; set; }
        public string CallingNumber { get; set; }
        public bool IsReservedSms { get; set; }
        public DateTime? ReservedTime { get; set; }
        public DateTime? ExecutionTime { get; set; }

        public SmsProgressStatus SmsProgressStatus { get; set; }

        public string MessageDispatchItemId { get; set; }

        public string SmsTraceId { get; set; }
        public string CallStatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public string SenderId { get; set; }

        public string FullName { get; set; }
        public string DepartmentId { get; set; }
        public string DisplayId { get; set; }
        public int? TotalCount { get; set; }
        public int? SuccessCount { get; set; }

        public SmsSendLogDto() { }

    }
}
