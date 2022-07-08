using CHIS.NotificationCenter.Application.Commands.CommunicationNote;
using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate;
using CHIS.NotificationCenter.Domain.Enum;
using CHIS.NotificationCenter.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.Framework.Middleware;
using CHIS.Framework.Core.Claims;
using CHIS.Framework.Core;
using CHIS.NotificationCenter.Domain.SeedWork;
using CHIS.Framework.Core.Localization;
using CHIS.Framework.Core.Extension.Messaging;
using CHIS.Share.AuditTrail.Services;
using System.Reflection;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.Enums;

namespace CHIS.NotificationCenter.Application.Commands.CommunicationNote
{

    public class ModifyCommunicationNoteReadStatusCommandHandler : IRequestHandler<ModifyCommunicationNoteReadStatusCommand, bool>
    {
        private readonly IEmployeeMessageBoxRepository _employeeMessageBoxRepository;
        //private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        //private readonly IOneSignalInterfaceService _oneSignalInterfaceService;
        //private readonly IMessageDispatchItemRepository _messageDispatcherRepository;
        //private readonly ICallContext _context;
        private readonly IUtcService _utcService;
        private readonly ITimeManager _timeManager;
        private readonly ILoggingService _loggingService;
        //private readonly ILocalizationManager _localizationManager;
        //private readonly IMessagingService _messagingService;

        public ModifyCommunicationNoteReadStatusCommandHandler(ICallContext context
            , IEmployeeMessageBoxRepository employeeMessageBoxRepository
            , IMessageSpecificationRepository messageSpecificationRepository
            , IMessageDispatchItemRepository messageDispatcherRepository
            , IOneSignalInterfaceService oneSignalInterfaceService
            , IUtcService utcService
            , ITimeManager timeManager
            , ILocalizationManager localizationManager
            , IMessagingService messagingService
            , ILoggingService loggingService
            )
        {
            //_context = context ?? throw new ArgumentNullException(nameof(context));
            _employeeMessageBoxRepository = employeeMessageBoxRepository ?? throw new ArgumentNullException(nameof(employeeMessageBoxRepository));
            //_messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            //_messageDispatcherRepository = messageDispatcherRepository ?? throw new ArgumentNullException(nameof(messageDispatcherRepository));
            //_oneSignalInterfaceService = oneSignalInterfaceService ?? throw new ArgumentNullException(nameof(oneSignalInterfaceService));
            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
            _timeManager = timeManager ?? throw new ArgumentException(nameof(timeManager));
            _loggingService = loggingService ?? throw new ArgumentException(nameof(loggingService));
            //_localizationManager = localizationManager ?? throw new ArgumentNullException(nameof(localizationManager));
            //_messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        }

        public async Task<bool> Handle(ModifyCommunicationNoteReadStatusCommand request, CancellationToken cancellationToken)
        {


            DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();
            UtcPack utcPack = _utcService.GetUtcPack(currentLocalDateTime);
            DateTime currentUtcDateTiem = _timeManager.GetUTCNow();

            if (request != null)
            {

                try
                {
                    CHIS.Share.AuditTrail.Model.Trace getTrace = _loggingService.GetTrace(Share.AuditTrail.Model.WhatEvent.Create, MethodBase.GetCurrentMethod().DeclaringType.FullName);
                    var updateTrace = new Trace(new WhoTrace(getTrace.WhoTrace.EmployeeId, getTrace.WhoTrace.EmployeeDisplayId, getTrace.WhoTrace.EmployeeName),
                        new WhenTrace(getTrace.WhenTrace.DateTime ?? currentLocalDateTime, getTrace.WhenTrace.TimeZoneId, getTrace.WhenTrace.DateTimeUtc ?? currentUtcDateTiem),
                        new WhereTrace(getTrace.WhereTrace.IpAddress, getTrace.WhereTrace.ViewId, getTrace.WhereTrace.MethodName),
                        new WhatTrace(WhatEvent.Update));

                    var employeeMessageInstance =
                                await _employeeMessageBoxRepository.RetrieveEmployeeMessageInstance(request.MessageInstanceId).ConfigureAwait(false);

                    employeeMessageInstance.IsReaded = true;
                    employeeMessageInstance.ReadTime = currentLocalDateTime;
                    employeeMessageInstance.ReadTimeUtcPack = utcPack;

                    employeeMessageInstance.IsHandled = true;
                    employeeMessageInstance.HandleTime = currentLocalDateTime;
                    employeeMessageInstance.HandleTimeUtcPack = utcPack;
                    employeeMessageInstance.DataLastModifiedDateTimeUtc = currentUtcDateTiem;
                    employeeMessageInstance.Trace = updateTrace;
                    _employeeMessageBoxRepository.UpdateEmployeeMessageInstance(employeeMessageInstance);
                    await _employeeMessageBoxRepository.UnitOfWork.SaveEntitiesWithMessagingAsync().ConfigureAwait(false);

                    return true;
                }
                catch (Exception e)
                {
                    return true;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(request));
            }
        }

    }
}
