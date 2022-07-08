using System;
using System.Collections.Generic;
using System.Text;
using CHIS.Framework.Core.Extension.Messaging.EventBus.Events;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events
{
    public class InboxMessageRecipientStatusChangedIntegrationEvent : IntegrationEvent
    {
        public string MessageDispatchItemId { get;  set; }

        public string ServiceCode { get; set; }

        public string ReferenceId { get; set; }

        public string EmployeeId { get; set; }

        public DateTime? HandleDateTime { get; set; }
        public DateTime? HandleDateTimeUtc { get; set; }
        public string TimeZoneId { get; set; }
        public string TenantId { get; set; }

        public string HospitalId { get; set; }

        public InboxMessageRecipientStatusChangedIntegrationEvent(
            string messageDispatchItemId, 
            string serviceCode, 
            string referenceId,
            string employeeId,
            DateTime? handleDateTime,
            DateTime? handleDateTimeUtc,
            string timeZoneId,
            string tenantId, 
            string hospitalId)
        {
            MessageDispatchItemId = messageDispatchItemId;
            ServiceCode = serviceCode;
            ReferenceId = referenceId;
            EmployeeId = employeeId;
            HandleDateTime = handleDateTime;
            HandleDateTimeUtc = handleDateTimeUtc;
            TimeZoneId = timeZoneId;
            TenantId = tenantId;
            HospitalId = hospitalId;
        } 
    }
}
