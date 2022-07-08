using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using MediatR;
namespace CHIS.NotificationCenter.Domain.Events
{
    public class SmsMessageDispatchStartedDomainEvent : INotification
    {
        public MessageDispatchItem MessageDispatchItem { get; set; }
        public SmsMessageDispatchStartedDomainEvent(MessageDispatchItem messageDispatchItem)
        {
            MessageDispatchItem = messageDispatchItem;
        }

    }
}
