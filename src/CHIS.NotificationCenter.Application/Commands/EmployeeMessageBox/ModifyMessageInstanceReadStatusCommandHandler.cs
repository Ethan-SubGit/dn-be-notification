using CHIS.Framework.Core;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events;
using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using CHIS.NotificationCenter.Domain.SeedWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using CHIS.Framework.Core.Extension.Messaging;
namespace CHIS.NotificationCenter.Application.Commands.EmployeeMessageBox
{
    public class ModifyMessageInstanceReadStatusCommandHandler : IRequestHandler<ModifyMessageInstanceReadStatusCommand, bool>
    {
        private readonly IEmployeeMessageBoxRepository _employeeMessageBoxRepository;
        //private readonly IEmployeeMessageBoxQueries _employeeMessageBoxQueries;
        private readonly IUtcService _utcService;
        //private readonly ICallContext _callContext;
        //private readonly IMessagingService _messagingService;
        //private readonly IMessageDispatchItemRepository _messageDispatcherRepository;

        public ModifyMessageInstanceReadStatusCommandHandler(IEmployeeMessageBoxRepository employeeMessageBoxRepository
            //, ICallContext callContext
            , IUtcService utcService
            //, IMessageDispatchItemRepository messageDispatcherRepository
            //, IMessageSpecificationRepository messageSpecificationRepository
            //, IEmployeeMessageBoxQueries employeeMessageBoxQueries
            //, IMessagingService messagingService
            )
        {
            _employeeMessageBoxRepository = employeeMessageBoxRepository ?? throw new ArgumentNullException(nameof(employeeMessageBoxRepository));
            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
            //_callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            //_messageDispatcherRepository = messageDispatcherRepository ?? throw new ArgumentNullException(nameof(messageDispatcherRepository));
            //_employeeMessageBoxQueries = employeeMessageBoxQueries ?? throw new ArgumentNullException(nameof(employeeMessageBoxQueries));
            //_messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));
        }

        public async Task<bool> Handle(ModifyMessageInstanceReadStatusCommand request, CancellationToken cancellationToken)
        {
            
            if (request == null || request.messageInstances == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();
            
            //InboxMessageRecipientStatusChangedIntegrationEvent inboxMessageRecipientStatusChangedIntegrationEvent = null;


            // DESC : 메시지 카테고리가 G(General) 일경우 isHandled까지 처리함.
            foreach (var messageInstance in request.messageInstances)
            {
                
                var employeeMessageInstance = await _employeeMessageBoxRepository.RetrieveEmployeeMessageInstance(messageInstance.Id).ConfigureAwait(false);

                if (employeeMessageInstance == null)
                {
                    continue;
                }

                //var messageCategory = await _employeeMessageBoxQueries.RetrieveMessageCategory(messageInstance.Id).ConfigureAwait(false);

                employeeMessageInstance.IsReaded = true;
                employeeMessageInstance.ReadTime = currentLocalDateTime;
                employeeMessageInstance.ReadTimeUtcPack = _utcService.GetUtcPack(currentLocalDateTime);


                //if (messageCategory.MessageCategory == "G")
                //{
                //    employeeMessageInstance.IsHandled = true;
                //    employeeMessageInstance.HandleTime = currentLocalDateTime;
                //    employeeMessageInstance.HandleTimeUtcPack = _utcService.GetUtcPack(currentLocalDateTime);
                //}
                _employeeMessageBoxRepository.UpdateEmployeeMessageInstance(employeeMessageInstance);


            }

            await _employeeMessageBoxRepository.UnitOfWork.SaveEntitiesWithMessagingAsync().ConfigureAwait(false);

            return true;
        }
    }
}