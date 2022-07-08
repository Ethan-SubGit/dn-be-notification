using CHIS.Framework.Core;
using CHIS.Framework.Core.Claims;
using CHIS.Framework.Core.Configuration;
using CHIS.Framework.Core.Extension.Messaging;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;

//using CHIS.Share.NotificationCenter.Services;
using CHIS.Share.NotificationCenter.Event;
using CHIS.Share.NotificationCenter.Models;
using CHIS.Share.NotificationCenter.Enum;


namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{

    public class SmsMqTestHandler : IRequestHandler<SmsMqTest, bool>
    {
        //private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        //private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        //private readonly ISmsMonitoringRepository _smsMonitoringRepository;
        //private readonly IMessageDispatcherQueries _messageDispatcherQueries;

        //private readonly ITimeManager _timeManager;
        //private readonly ICallContext _callContext;
        //private readonly ISmsService _smsService;
        //private readonly Share.NotificationCenter.Services.ISmsService _smsShareService;
        private readonly IMessagingService _messagingService;

        // Using DI to inject infrastructure persistence Repositories
        public SmsMqTestHandler(
            IMessageDispatchItemRepository messageDispatchItemRepository
            //, IMessageSpecificationRepository messageSpecificationRepository
            //, ISmsMonitoringRepository smsMonitoringRepository
            //, IMessageDispatcherQueries messageDispatcherQueries
            //, ICallContext callContext
            //, ITimeManager timeManager
            //, ISmsService smsService
            //, IMapper mapper
            //, Share.NotificationCenter.Services.ISmsService smsShareService
            , IMessagingService messagingService
            )
        {
            //_messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            //_messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            //_smsMonitoringRepository = smsMonitoringRepository ?? throw new ArgumentNullException(nameof(smsMonitoringRepository));
            //_messageDispatcherQueries = messageDispatcherQueries ?? throw new ArgumentNullException(nameof(messageDispatcherQueries));
            //_timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            //_callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            //_smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));

            //_smsShareService = smsShareService ?? throw new ArgumentNullException(nameof(smsShareService));
        }

        public async Task<bool> Handle(SmsMqTest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }

            //string tenantId = _callContext.TenantId;
            //string hospitalId = _callContext.HospitalId;

            if (request.test == "1")
            {
                PatientSmsMessageSendIntegrationEvent patientSmsMessageSendEvent = new PatientSmsMessageSendIntegrationEvent();
                patientSmsMessageSendEvent.PatientSmsMessages.Add(
                    new Share.NotificationCenter.Models.PatientSmsMessageDto()
                    {
                        ServiceCode = "SMS00001",
                        MessageTemplateId = "c29c1f7d-bc99-4cb7-b433-8ad8cc76e46d",
                        SenderId = "System",
                        Content = "환자 SMS 발송 {0} 테스트 파라미터",
                        IsUsingPredefinedContent = false,
                        IsReservedSms = false,
                        ReservedSmsDateTime = DateTime.Now,
                        PatientId = request.PatientId,
                        SmsRecipientType = Share.NotificationCenter.Enum.SmsRecipientType.Patient,
                        ContactClassificationCode = string.Empty,
                        ContactRelationShipCode = string.Empty,
                        ContentParameters = new List<ContentParameterDto>() {
                            new ContentParameterDto() { ParameterValue = "TEST1" }
                        , new ContentParameterDto() { ParameterValue = "TEST2" } },
                        Mobile = ""
                    });

                await _messagingService.SendAsync(patientSmsMessageSendEvent, "notificationcenter").ConfigureAwait(false);
            }

        

            return true;
        }
    }
}
