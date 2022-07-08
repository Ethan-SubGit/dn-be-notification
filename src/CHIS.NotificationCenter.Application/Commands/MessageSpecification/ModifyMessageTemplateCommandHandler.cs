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
    public class ModifyMessageTemplateCommandHandler : IRequestHandler<ModifyMessageTemplateCommand, bool>
    {
        private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        //private readonly ICallContext _callContext;
        private readonly ILoggingService _loggingService;
        private readonly ITimeManager _timeManager;

        public ModifyMessageTemplateCommandHandler(IMessageSpecificationRepository messageSpecificationRepository
            //, ICallContext callContext
            , ITimeManager timeManager
            , ILoggingService loggingService)
        {
            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            //_callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _timeManager = timeManager ?? throw new ArgumentException(nameof(timeManager));
        }
        public async Task<bool> Handle(ModifyMessageTemplateCommand request, CancellationToken cancellationToken)
        {
            DateTime currentUtcDateTime = _timeManager.GetUTCNow();
            CHIS.Share.AuditTrail.Model.Trace getTrace = _loggingService.GetTrace(Share.AuditTrail.Model.WhatEvent.Create, MethodBase.GetCurrentMethod().DeclaringType.FullName);

            var modifyTrace = new Trace(new WhoTrace(getTrace.WhoTrace.EmployeeId, getTrace.WhoTrace.EmployeeDisplayId, getTrace.WhoTrace.EmployeeName),
                new WhenTrace(getTrace.WhenTrace.DateTime ?? currentUtcDateTime, getTrace.WhenTrace.TimeZoneId, getTrace.WhenTrace.DateTimeUtc ?? currentUtcDateTime),
                new WhereTrace(getTrace.WhereTrace.IpAddress, getTrace.WhereTrace.ViewId, getTrace.WhereTrace.MethodName),
                new WhatTrace(WhatEvent.Create));

            if (request != null)
            {
                MessageTemplate messageTemplate =  _messageSpecificationRepository.RetrieveTemplate(id: request.Id);

                messageTemplate.TemplateTitle = request.TemplateTitle;
                messageTemplate.DataLastModifiedDateTimeUtc = currentUtcDateTime;
                messageTemplate.ContentTemplate = request.ContentTemplate;
                messageTemplate.ContentTemplateScope = request.ContentTemplateScope;
                messageTemplate.MessageSpecificationId = request.MessageSpecificationId;
                messageTemplate.Trace = modifyTrace;

                _messageSpecificationRepository.UpdateTemplate(messageTemplate: messageTemplate);
                await _messageSpecificationRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);

                return true;

            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
