using CHIS.NotificationCenter.Application.Commands.MessageDispatcher;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CHIS.Framework.Core;
using CHIS.Framework.Middleware;
using CHIS.Share.NotificationCenter.Models;
using CHIS.NotificationCenter.Application.Services;
using System.Reflection;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.Enums;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    /// <summary>
    /// Inbox 메시지 취소기능
    /// </summary>
    public class RequestCancellationOfInboxMessageNotificationCommandHandler : IRequestHandler<RequestCancellationOfInboxMessageNotificationCommand, int>
    {
        private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        private readonly IEmployeeMessageBoxRepository _employeeMessageBoxRepository;
        //private readonly IMessageSpecificationRepository _messageSpecificationRepository;

        private readonly ITimeManager _timeManager;
        //private readonly ICallContext _callContext;
        //private readonly ISmsService _smsService;
        private readonly CHIS.Share.AuditTrail.Services.ILoggingService _loggingService; //AuditTrail Logging

        // Using DI to inject infrastructure persistence Repositories
        public RequestCancellationOfInboxMessageNotificationCommandHandler(IMessageDispatchItemRepository messageDispatchItemRepository,
            IEmployeeMessageBoxRepository employeeMessageBoxRepository
            //, IMessageSpecificationRepository messageSpecificationRepository
            //, ICallContext callContext
            , ITimeManager timeManager
            //, ISmsService smsService
            , CHIS.Share.AuditTrail.Services.ILoggingService loggingService //AuditTrail Logging
            )
        {
            _messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            _employeeMessageBoxRepository = employeeMessageBoxRepository ?? throw new ArgumentNullException(nameof(employeeMessageBoxRepository));
            //_messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            //_callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            //_smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService)); //AuditTrail Logging
        }

        public async Task<int> Handle(RequestCancellationOfInboxMessageNotificationCommand request, CancellationToken cancellationToken)
        {
            if (request != null)
            {

                DateTime currentUtcDateTime = _timeManager.GetUTCNow();

                CHIS.Share.AuditTrail.Model.Trace getTrace = _loggingService.GetTrace(Share.AuditTrail.Model.WhatEvent.Create, MethodBase.GetCurrentMethod().DeclaringType.FullName);

                var updateTrace = new Trace(new WhoTrace(getTrace.WhoTrace.EmployeeId, getTrace.WhoTrace.EmployeeDisplayId, getTrace.WhoTrace.EmployeeName),
                    new WhenTrace(getTrace.WhenTrace.DateTime ?? currentUtcDateTime, getTrace.WhenTrace.TimeZoneId, getTrace.WhenTrace.DateTimeUtc ?? currentUtcDateTime),
                    new WhereTrace(getTrace.WhereTrace.IpAddress, getTrace.WhereTrace.ViewId, getTrace.WhereTrace.MethodName),
                    new WhatTrace(WhatEvent.Update));


                //[1] 이미 읽은 메시지가 있는지 체크 
                var handledCount =  _employeeMessageBoxRepository.GetUnhandledEmployeeMessageInstanceCountByMessageDispatchItemId(messageDispatchItemId: request.MessageDispatchItemId);
                if (handledCount > 0)
                {
                    return 0;
                }


                var messageDispatchItem = await _messageDispatchItemRepository.Retrieve(request.MessageDispatchItemId).ConfigureAwait(false);
                messageDispatchItem.IsCanceled = true;
                messageDispatchItem.DataLastModifiedDateTimeUtc = currentUtcDateTime;
                messageDispatchItem.Trace = updateTrace;

                var returnCount = await _messageDispatchItemRepository.UnitOfWork.SaveChangesAsync().ConfigureAwait(false);
                
                return returnCount;
            }
            else
            {
                throw new ArgumentNullException(nameof(request));
            }
        }
    }
}
