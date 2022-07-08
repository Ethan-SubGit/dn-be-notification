using CHIS.Framework.Core.Extension.Messaging.EventBus.Events;
using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events.Model;
namespace CHIS.NotificationCenter.Application.IntegrationMessages.Events
{
    /// <summary>
    /// read model 로 변경하면서 MQ 수신하지 않음.
    /// 2020.1
    /// </summary>
    public class JobPositionCreatedIntegrationEvent : IntegrationEvent
    {
        public JobPositionDto JobPosition { get; }
        public string TenantId { get; }

        public JobPositionCreatedIntegrationEvent(JobPositionDto jobPosition, string tenantId)
        {
            JobPosition = jobPosition;
            TenantId = tenantId;
        }
    }
}
