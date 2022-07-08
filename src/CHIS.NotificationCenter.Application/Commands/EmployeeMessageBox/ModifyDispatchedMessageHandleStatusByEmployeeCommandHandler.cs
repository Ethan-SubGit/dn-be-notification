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
    /// EmployeeId 로 특정 메시지 확인 처리. (by messageDispatchItemId and employeeId)
    /// Examination 제공코드
    /// </summary>
    public class ModifyDispatchedMessageHandleStatusByEmployeeCommandHandler : IRequestHandler<ModifyDispatchedMessageHandleStatusByEmployeeCommand, bool>
    {

        private readonly IEmployeeMessageBoxRepository _employeeMessageBoxRepository;
        private readonly IMessageDispatchItemRepository _messageDispatcherRepository;
        private readonly IUtcService _utcService;
        private readonly IMessagingService _messagingService;

        public ModifyDispatchedMessageHandleStatusByEmployeeCommandHandler(ICallContext context
            , IEmployeeMessageBoxRepository employeeMessageBoxRepository
            , IMessageDispatchItemRepository messageDispatcherRepository
            , IUtcService utcService
            , IMessagingService messagingService
            )
        {
            _employeeMessageBoxRepository = employeeMessageBoxRepository ?? throw new ArgumentNullException(nameof(employeeMessageBoxRepository));
            _messageDispatcherRepository = messageDispatcherRepository ?? throw new ArgumentNullException(nameof(messageDispatcherRepository));
            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        }

        public async Task<bool> Handle(ModifyDispatchedMessageHandleStatusByEmployeeCommand request, CancellationToken cancellationToken)
        {
            
            if (request != null)
            {

                DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();
                UtcPack utcPack = _utcService.GetUtcPack(currentLocalDateTime);

                var messageDispatchItem = await _messageDispatcherRepository.Retrieve(request.MessageDispatchItemId).ConfigureAwait(false);
                var employeeMessageBox = await _employeeMessageBoxRepository.RetrieveByEmployeeId(request.EmployeeId).ConfigureAwait(false);

                var employeeMessageInstance = 
                    await _employeeMessageBoxRepository.RetrieveEmployeeMessageInstance(employeeMessageBox.Id, request.MessageDispatchItemId , false , true).ConfigureAwait(false); 

                //이미 handled 처리된 경우 진입하지 않음
                if (employeeMessageInstance != null && !employeeMessageInstance.IsHandled)
                {
                    employeeMessageInstance.IsHandled = true;
                    employeeMessageInstance.HandleTime = currentLocalDateTime;
                    employeeMessageInstance.HandleTimeUtcPack = utcPack;
                    _employeeMessageBoxRepository.UpdateEmployeeMessageInstance(employeeMessageInstance);

                    InboxMessageRecipientStatusChangedIntegrationEvent inboxMessageRecipientStatusChangedIntegrationEvent =
                      new InboxMessageRecipientStatusChangedIntegrationEvent(request.MessageDispatchItemId, 
                        messageDispatchItem.ServiceCode,
                          messageDispatchItem.ReferenceId,
                          employeeMessageInstance.EmployeeId,
                          currentLocalDateTime,
                          utcPack.DateTimeUtc,
                          utcPack.TimeZoneId,
                          messageDispatchItem.TenantId, 
                          messageDispatchItem.HospitalId);
                    
                    await _messagingService.AddSequenceCandidateAsync(inboxMessageRecipientStatusChangedIntegrationEvent, "MessageDispatchItemId", request.MessageDispatchItemId).ConfigureAwait(false);
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
