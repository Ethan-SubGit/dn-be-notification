using System;
using System.Collections.Generic;
using System.Text;
using CHIS.Framework.Core.Extension.Messaging.EventBus.Events;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model;
namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events
{
    /// <summary>
    /// read model 로 변경하면서 MQ 수신하지 않음.
    /// 2020.1
    /// </summary>
    public class DepartmentCreatedIntegrationEvent : IntegrationEvent
    {
        public DepartmentDto Department { get; }
        public string TenantId { get; }
        public string HospitalId { get; }

        public DepartmentCreatedIntegrationEvent(DepartmentDto department, string tenantId, string hospitalId)
        {
            Department = department;
            TenantId = tenantId;
            HospitalId = hospitalId;

        }
    }
}
