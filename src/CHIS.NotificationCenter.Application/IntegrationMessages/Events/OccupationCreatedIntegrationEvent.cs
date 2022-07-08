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
    public class OccupationCreatedIntegrationEvent : IntegrationEvent
    {
        public OccupationDto Occupation { get; }
        public string TenantId { get; }

        public OccupationCreatedIntegrationEvent(OccupationDto occupation, string tenantId)
        {
            Occupation = occupation;
            TenantId = tenantId;
        }
    }
}
