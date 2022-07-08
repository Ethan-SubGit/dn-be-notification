using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model;
using CHIS.Framework.Core.Extension.Messaging.EventBus.Events;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events
{
    /// <summary>
    /// read model 로 변경하면서 MQ 수신하지 않음.
    /// 2020.1
    /// </summary>
    public class EmployeeRegisteredIntegrationEvent : IntegrationEvent
    {
        public EmployeeDto Employee { get; private set; }
        public string TenantId { get; private set; }
        public string HospitalId { get; private set; }

        public EmployeeRegisteredIntegrationEvent(EmployeeDto employee, string tenantId, string hospitalId)
        {
            this.Employee = employee;
            this.TenantId = tenantId;
            this.HospitalId = hospitalId;
        }
    }
}
