using CHIS.Framework.Core;
using CHIS.Framework.Core.Claims;
using CHIS.Framework.Core.Configuration;
using CHIS.Framework.Core.Extension.Messaging;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Application.Models.CommonModels;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Application.Commands.MessageDispatcher;
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
using CHIS.NotificationCenter.Application.Commands.SmsMonitoring;

//using CHIS.Share.NotificationCenter.Services;
//using CHIS.Share.NotificationCenter.Event;
//using CHIS.Share.NotificationCenter.Models;
//using CHIS.Share.NotificationCenter.Enum;


namespace CHIS.NotificationCenter.Application.Commands.SmsMonitoring
{
    public class ResendPatientSmsCommandHandler : IRequestHandler<ResendPatientSmsCommand, bool>
    {
        private readonly IMediator _mediator;
        //private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        //private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        private readonly ISmsMonitoringRepository _smsMonitoringRepository;
        //private readonly IMessageDispatcherQueries _messageDispatcherQueries;

        //private readonly ITimeManager _timeManager;
        //private readonly ICallContext _callContext;
        // private readonly ISmsService _smsService;
        //private readonly Share.NotificationCenter.Services.ISmsService _smsShareService;
        //private readonly IMessagingService _messagingService;
        // Using DI to inject infrastructure persistence Repositories
        public ResendPatientSmsCommandHandler(
            IMediator mediator
            //, IMessageDispatchItemRepository messageDispatchItemRepository
            //, IMessageSpecificationRepository messageSpecificationRepository
            , ISmsMonitoringRepository smsMonitoringRepository
            //, IMessageDispatcherQueries messageDispatcherQueries
            //, ICallContext callContext
            //, ITimeManager timeManager
            //ISmsService smsService
            //, Share.NotificationCenter.Services.ISmsService smsShareService
            //, IMessagingService messagingService
            )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            //_messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            _smsMonitoringRepository = smsMonitoringRepository ?? throw new ArgumentNullException(nameof(smsMonitoringRepository));
            //_messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            //_messageSentLogRepository = messageSentLogRepository ?? throw new ArgumentNullException(nameof(messageSentLogRepository));
            //_messageDispatcherQueries = messageDispatcherQueries ?? throw new ArgumentNullException(nameof(messageDispatcherQueries));
            //_timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            //_callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            //_smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            //_messagingService = messagingService ?? throw new ArgumentNullException(nameof(messagingService));

            //_smsShareService = smsShareService ?? throw new ArgumentNullException(nameof(smsShareService));
        }

        public async Task<bool> Handle(ResendPatientSmsCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return await Task.FromResult(false).ConfigureAwait(false);
            }

            //string tenantId = _callContext.TenantId;
            //string hospitalId = _callContext.HospitalId;
            RequestPatientSmsMessageNotificationCommand requestPatientSmsMessageNotificationCommand = null;
            PatientSmsMessageDto patientSmsMessage = null;
            try
            {
                foreach (var messageDispatchItemId in request.MessageDispatchItemIds)
                {
                    requestPatientSmsMessageNotificationCommand = new RequestPatientSmsMessageNotificationCommand();

                    //var messageDispatchItem = await _messageDispatchItemRepository.Retrieve(messageDispatchItemId).ConfigureAwait(false);
                    var smsSendLog = await _smsMonitoringRepository.FindSmsSendLogByMessageDispatchItemId(messageDispatchItemId).ConfigureAwait(false);
                    var smsReceiveLogs = await _smsMonitoringRepository.FindSmsReceiveLogByMessageDispatchItemId(messageDispatchItemId).ConfigureAwait(false);

                    foreach (var receiveLog in smsReceiveLogs)
                    {
                        patientSmsMessage = new PatientSmsMessageDto();
                        patientSmsMessage.PatientId = receiveLog.ActorId;
                        patientSmsMessage.Content = receiveLog.Content;
                        patientSmsMessage.IsReservedSms = false;
                        patientSmsMessage.SenderId = smsSendLog.SenderId;
                        patientSmsMessage.ServiceCode = smsSendLog.ServiceCode;
                        patientSmsMessage.SmsRecipientType = SmsRecipientType.Patient;


                        requestPatientSmsMessageNotificationCommand.SmsMessages.Add(patientSmsMessage);
                    }

                    if (requestPatientSmsMessageNotificationCommand.SmsMessages.Count > 0)
                    {
                        await _mediator.Send(requestPatientSmsMessageNotificationCommand).ConfigureAwait(false);
                    }


                    //smsRecipientDto


                    //requestPatientSmsMessageNotificationCommand.SmsMessages.Add()
                    //await _smsService.ResendPatientSmsMessage(messageDispatchItemId).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                return false;
            }
           

            return true;
        }
    }
}
