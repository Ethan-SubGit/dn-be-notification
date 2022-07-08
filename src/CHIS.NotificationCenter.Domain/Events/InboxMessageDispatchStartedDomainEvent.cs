using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using MediatR;
namespace CHIS.NotificationCenter.Domain.Events
{
    public class InboxMessageDispatchStartedDomainEvent : INotification 
    {
        public MessageDispatchItem MessageDispatchItem { get; set; } 
        public InboxMessageDispatchStartedDomainEvent(MessageDispatchItem messageDispatchItem)
        {
            MessageDispatchItem = messageDispatchItem;
        }

    }
}
