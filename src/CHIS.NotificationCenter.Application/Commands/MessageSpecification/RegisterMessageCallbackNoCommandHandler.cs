using CHIS.Framework.Core;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.Enums;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.Share.AuditTrail.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Commands.MessageSpecification
{
    public class RegisterMessageCallbackNoCommandHandler : IRequestHandler<RegisterMessageCallbackNoCommand, string>
    {
        private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        private readonly ICallContext _callContext;
        private readonly ILoggingService _loggingService;
        private readonly ITimeManager _timeManager;

        public RegisterMessageCallbackNoCommandHandler(IMessageSpecificationRepository messageSpecificationRepository
            , ICallContext callContext
            , ITimeManager timeManager
            , ILoggingService loggingService)
        {
            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _timeManager = timeManager ?? throw new ArgumentException(nameof(timeManager));
        }

        public async Task<string> Handle(RegisterMessageCallbackNoCommand request, CancellationToken cancellationToken)
        {
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            DateTime currentUtcDateTime = _timeManager.GetUTCNow();
            CHIS.Share.AuditTrail.Model.Trace getTrace = _loggingService.GetTrace(Share.AuditTrail.Model.WhatEvent.Create, MethodBase.GetCurrentMethod().DeclaringType.FullName);

            var createTrace = new Trace(new WhoTrace(getTrace.WhoTrace.EmployeeId, getTrace.WhoTrace.EmployeeDisplayId, getTrace.WhoTrace.EmployeeName),
                new WhenTrace(getTrace.WhenTrace.DateTime ?? currentUtcDateTime, getTrace.WhenTrace.TimeZoneId, getTrace.WhenTrace.DateTimeUtc ?? currentUtcDateTime),
                new WhereTrace(getTrace.WhereTrace.IpAddress, getTrace.WhereTrace.ViewId, getTrace.WhereTrace.MethodName),
                new WhatTrace(WhatEvent.Create));

            if (request != null)
            {
                MessageCallbackNoConfig messageCallbackNoConfig = new MessageCallbackNoConfig(
                    callbackTitle: request.CallbackTitle
                    ,callbackNo: request.CallbackNo
                    ,description: request.Description
                    ,tenantId: tenantId
                    ,hospitalId: hospitalId
                    ,dataFirstRegisteredDateTimeUtc: currentUtcDateTime
                    ,dataLastModifiedDateTimeUtc: currentUtcDateTime
                    ,trace: createTrace
                    ,isDeleted: request.IsDeleted
                    ,isSystemProperty: request.IsSystemProperty
                    ,isMaster: request.IsMaster
                    );
                messageCallbackNoConfig = _messageSpecificationRepository.CreateCallbackNo(messageCallbackNoConfig);
                await _loggingService.PrepareMessageWithAuditTrailAsync(
                    messageCallbackNoConfig, "1.0.0", Share.AuditTrail.Enum.TypeOfActionType.Addition, Share.AuditTrail.Enum.EventCoverageType.Full).ConfigureAwait(false);

                await _messageSpecificationRepository.UnitOfWork.SaveEntitiesWithMessagingAsync().ConfigureAwait(false);

                return messageCallbackNoConfig.Id;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
