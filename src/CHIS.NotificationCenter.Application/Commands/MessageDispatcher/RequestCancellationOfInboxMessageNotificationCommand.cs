using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Newtonsoft.Json;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using System.Dynamic;
using CHIS.Share.NotificationCenter.Models;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    public class RequestCancellationOfInboxMessageNotificationCommand : IRequest<int>
    {
        public string MessageDispatchItemId { get; set; }
    }
}
