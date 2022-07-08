using CHIS.Framework.Core;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox;
using CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using CHIS.NotificationCenter.Domain.SeedWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events;
using CHIS.Framework.Core.Extension.Messaging;
using CHIS.NotificationCenter.Application.Services;

namespace CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox
{
    /// <summary>
    /// 특정 메시지 확인 처리함 (By Message Instance Id)
    /// </summary>
    public class ModifyMessageInstanceHandleStatusCommandHandler : IRequestHandler<ModifyMessageInstanceHandleStatusCommand, bool>
    {
        private readonly IEmployeeMessageBoxRepository _employeeMessageBoxRepository;

        private readonly IUtcService _utcService;
        private readonly ITimeManager _timeManager;
        //private readonly ICallContext _callContext;
        //private readonly IMessagingService _messagingService;
        private readonly IMessageDispatchItemRepository _messageDispatcherRepository;
        private readonly IMessageSpecificationRepository _messageSpecificationRepository;

        public ModifyMessageInstanceHandleStatusCommandHandler(IEmployeeMessageBoxRepository employeeMessageBoxRepository
            //, ICallContext callContext
            , IMessageSpecificationRepository messageSpecificationRepository
            , IUtcService utcService
            , ITimeManager timeManager
            , IMessageDispatchItemRepository messageDispatcherRepository
            , IMessagingService messagingService)
        {
            _employeeMessageBoxRepository = employeeMessageBoxRepository ?? throw new ArgumentNullException(nameof(employeeMessageBoxRepository));
            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            //_callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            _messageDispatcherRepository = messageDispatcherRepository ?? throw new ArgumentNullException(nameof(messageDispatcherRepository));
            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            //_messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        }

        public async Task<bool> Handle(ModifyMessageInstanceHandleStatusCommand request, CancellationToken cancellationToken)
        {
            //InboxMessageRecipientStatusChangedIntegrationEvent inboxMessageRecipientStatusChangedIntegrationEvent = null;

            if (request == null || request.messageInstances == null)
            {
                throw new ArgumentNullException(nameof(request));
            }


            DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();
            UtcPack utcPack = _utcService.GetUtcPack(currentLocalDateTime);
            DateTime currentUtcDateTime = _timeManager.GetUTCNow();
            

            //신규코드
            foreach (var messageInstance in request.messageInstances)
            {
                //var messageDispatchItem = await _messageDispatcherRepository.Retrieve(messageInstance.MessageDispatchItemId).ConfigureAwait(false);
                var messageDispatchItem = await _messageDispatcherRepository.RetrieveWithNoPolicy(messageInstance.MessageDispatchItemId).ConfigureAwait(false);
                var messageSpecification = _messageSpecificationRepository.FindByServiceCode(messageDispatchItem.ServiceCode);

                //messageIntance, messageSpecification join으로 변경해서 가져오는 방향으로
                /*
                    messageDispatchItemId, employeeMessageInstances
                */
                if (messageSpecification.PostActionType == Domain.Enum.PostActionType.ConfirmAll)
                {
                    //전체확인
                    var employeeMessageInstances =
                        await _employeeMessageBoxRepository.RetrieveUnhandledEmployeeMessageInstancesByMessageDispatchItemId(messageInstance.MessageDispatchItemId).ConfigureAwait(false);

                    if (employeeMessageInstances == null)
                    {
                        continue;
                    }

                    foreach (var employeeMessageInstance in employeeMessageInstances.FindAll(p => !p.IsHandled))
                    {
                        employeeMessageInstance.IsHandled = true;
                        employeeMessageInstance.HandleTime = currentLocalDateTime;
                        employeeMessageInstance.HandleTimeUtcPack = utcPack;
                        employeeMessageInstance.DataLastModifiedDateTimeUtc = currentUtcDateTime;
                        employeeMessageInstance.IsReaded = true;
                        employeeMessageInstance.ReadTime = currentLocalDateTime;
                        //employeeMessageInstance.ReadTimeUtcPack.TimeZoneId = timeZoneId;
                        //employeeMessageInstance.ReadTimeUtcPack.DateTimeUtc = currentUtcDateTime;

                        _employeeMessageBoxRepository.UpdateEmployeeMessageInstance(employeeMessageInstance);

                        #region ### 중복호출 의심
                        /*
                        //DESC : 상태변경 Event Firing 
                        inboxMessageRecipientStatusChangedIntegrationEvent = new InboxMessageRecipientStatusChangedIntegrationEvent
                            (employeeMessageInstance.MessageDispatchItemId,
                            messageDispatchItem.ServiceCode,
                            messageDispatchItem.ReferenceId,
                            employeeMessageInstance.EmployeeId,
                            currentLocalDateTime,
                            utcPack.DateTimeUtc,
                            utcPack.TimeZoneId,
                            employeeMessageInstance.TenantId,
                            employeeMessageInstance.HospitalId);

                        await _messagingService.PrepareMessageAsync(inboxMessageRecipientStatusChangedIntegrationEvent).ConfigureAwait(false); 
                        */
                        #endregion
                    }
                }
                else if (messageSpecification.PostActionType == Domain.Enum.PostActionType.Confirm)
                {
                    // DESC : 자기자신만 확인 처리 하는 경우
                    var employeeMessageInstance = await _employeeMessageBoxRepository.RetrieveEmployeeMessageInstance(messageInstance.Id).ConfigureAwait(false);

                    if (employeeMessageInstance == null || employeeMessageInstance.IsHandled)
                    {
                        continue;
                    }

                    employeeMessageInstance.IsHandled = true;
                    employeeMessageInstance.HandleTime = currentLocalDateTime;
                    employeeMessageInstance.HandleTimeUtcPack = utcPack;
                    employeeMessageInstance.DataLastModifiedDateTimeUtc = currentUtcDateTime;
                    employeeMessageInstance.IsReaded = true;
                    employeeMessageInstance.ReadTime = currentLocalDateTime;
                    //employeeMessageInstance.ReadTimeUtcPack.TimeZoneId = timeZoneId;
                    //employeeMessageInstance.ReadTimeUtcPack.DateTimeUtc = currentUtcDateTime;

                    _employeeMessageBoxRepository.UpdateEmployeeMessageInstance(employeeMessageInstance);


                    #region ### 중복호출 의심
                    /*
                    //DESC : 상태변경 Event Firing 
                    inboxMessageRecipientStatusChangedIntegrationEvent = new InboxMessageRecipientStatusChangedIntegrationEvent
                        (employeeMessageInstance.MessageDispatchItemId,
                        messageDispatchItem.ServiceCode,
                        messageDispatchItem.ReferenceId,
                        employeeMessageInstance.EmployeeId,
                        currentLocalDateTime,
                        utcPack.DateTimeUtc,
                        utcPack.TimeZoneId,
                        employeeMessageInstance.TenantId,
                        employeeMessageInstance.HospitalId);
                    await _messagingService.PrepareMessageAsync(inboxMessageRecipientStatusChangedIntegrationEvent).ConfigureAwait(false);
                    */
                    #endregion
                }
                else if (messageSpecification.PostActionType == Domain.Enum.PostActionType.Integration)
                {
                    // DESC : 자기자신만 확인 처리 하는 경우
                    var employeeMessageInstance = await _employeeMessageBoxRepository.RetrieveEmployeeMessageInstance(messageInstance.Id).ConfigureAwait(false);

                    if (employeeMessageInstance == null || employeeMessageInstance.IsHandled)
                    {
                        continue;
                    }

                    employeeMessageInstance.IsHandled = true;
                    employeeMessageInstance.HandleTime = currentLocalDateTime;
                    employeeMessageInstance.HandleTimeUtcPack = utcPack;
                    employeeMessageInstance.DataLastModifiedDateTimeUtc = currentUtcDateTime;
                    employeeMessageInstance.IsReaded = true;
                    employeeMessageInstance.ReadTime = currentLocalDateTime;
                    //employeeMessageInstance.ReadTimeUtcPack.TimeZoneId = timeZoneId;
                    //employeeMessageInstance.ReadTimeUtcPack.DateTimeUtc = currentUtcDateTime;

                    _employeeMessageBoxRepository.UpdateEmployeeMessageInstance(employeeMessageInstance);
                }



            }

            await _employeeMessageBoxRepository.UnitOfWork.SaveEntitiesWithMessagingAsync().ConfigureAwait(false);

            return true;
        }
    }
}