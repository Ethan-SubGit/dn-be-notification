using CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox;
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
using CHIS.NotificationCenter.Application.IntegrationMessages.Events;
using CHIS.Framework.Core.Extension.Messaging;

namespace CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox
{
    /// <summary>
    /// 전체 수신자 확인처리함. (Cosign Case)
    /// </summary>
    public class ModifyDispatchedMessageHandleStatusCommandHandler : IRequestHandler<ModifyDispatchedMessageHandleStatusCommand, bool>
    {
        private readonly IEmployeeMessageBoxRepository      _employeeMessageBoxRepository;
        //private readonly IMessageSpecificationRepository    _messageSpecificationRepository;
        //private readonly IOneSignalInterfaceService         _oneSignalInterfaceService;
        private readonly IMessageDispatchItemRepository     _messageDispatcherRepository;
        //private readonly ICallContext                       _context;
        private readonly IUtcService _utcService;
        //private readonly ILocalizationManager               _localizationManager;
        private readonly IMessagingService _messagingService;

        public ModifyDispatchedMessageHandleStatusCommandHandler(ICallContext context
            , IEmployeeMessageBoxRepository   employeeMessageBoxRepository
            , IMessageSpecificationRepository messageSpecificationRepository
            , IMessageDispatchItemRepository  messageDispatcherRepository
            , IOneSignalInterfaceService      oneSignalInterfaceService
            , IUtcService utcService
            , ILocalizationManager            localizationManager
            , IMessagingService messagingService
            )
        {
            //_context                        = context                       ?? throw new ArgumentNullException(nameof(context                       ));
            _employeeMessageBoxRepository   = employeeMessageBoxRepository  ?? throw new ArgumentNullException(nameof(employeeMessageBoxRepository  ));
            //_messageSpecificationRepository = messageSpecificationRepository?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _messageDispatcherRepository    = messageDispatcherRepository   ?? throw new ArgumentNullException(nameof(messageDispatcherRepository   ));
            //_oneSignalInterfaceService      = oneSignalInterfaceService     ?? throw new ArgumentNullException(nameof(oneSignalInterfaceService     ));
            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
            //_localizationManager            = localizationManager           ?? throw new ArgumentNullException(nameof(localizationManager           ));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        }

        public async Task<bool> Handle(ModifyDispatchedMessageHandleStatusCommand request, CancellationToken cancellationToken)
        {
            InboxMessageRecipientStatusChangedIntegrationEvent inboxMessageRecipientStatusChangedIntegrationEvent = null;

            DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();
            UtcPack utcPack = _utcService.GetUtcPack(currentLocalDateTime);


            if (request != null)
            {
                
                var messageDispatchItem = await _messageDispatcherRepository.Retrieve(request.MessageDispatchItemId).ConfigureAwait(false);
              

                var employeeMessageInstances =
                    await _employeeMessageBoxRepository.RetrieveUnhandledEmployeeMessageInstancesByMessageDispatchItemId(request.MessageDispatchItemId).ConfigureAwait(false);

                //IsHandled is false 인 경우만 처리
                foreach (EmployeeMessageInstance employeeMessageInstance in employeeMessageInstances.Where(p => !p.IsHandled))
                {
                    employeeMessageInstance.IsHandled = true;
                    employeeMessageInstance.HandleTime = currentLocalDateTime;
                    employeeMessageInstance.HandleTimeUtcPack = utcPack;
                    _employeeMessageBoxRepository.UpdateEmployeeMessageInstance(employeeMessageInstance);

                    // DESC : 각 employee 상태 변경에 따른 이벤트 firing
                    inboxMessageRecipientStatusChangedIntegrationEvent =
                        new InboxMessageRecipientStatusChangedIntegrationEvent(request.MessageDispatchItemId,
                        messageDispatchItem.ServiceCode,
                        messageDispatchItem.ReferenceId,
                        employeeMessageInstance.EmployeeId,
                        currentLocalDateTime,
                        utcPack.DateTimeUtc,
                        utcPack.TimeZoneId,
                        messageDispatchItem.TenantId,
                        messageDispatchItem.HospitalId);
                    await _messagingService.PrepareMessageAsync(inboxMessageRecipientStatusChangedIntegrationEvent).ConfigureAwait(false);
                }
                

                await _employeeMessageBoxRepository.UnitOfWork.SaveEntitiesWithMessagingAsync().ConfigureAwait(false);
             
                return true;
            }
            else
            {
                throw new ArgumentNullException(nameof(request));
            }
        }

    }
}

