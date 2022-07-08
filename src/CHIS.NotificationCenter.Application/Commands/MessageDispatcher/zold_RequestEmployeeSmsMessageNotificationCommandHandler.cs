//using CHIS.Framework.Core;
//using CHIS.Framework.Core.Claims;
//using CHIS.Framework.Core.Configuration;
//using CHIS.Framework.Core.Extension.Messaging;
//using CHIS.Framework.Middleware;
//using CHIS.NotificationCenter.Application.Models;
//using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
//using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
//using CHIS.NotificationCenter.Domain.Enum;
////using CHIS.Share.NotificationCenter.Models;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using CHIS.NotificationCenter.Application.Services;
//using CHIS.NotificationCenter.Application.Queries;
//using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
//using CHIS.NotificationCenter.Application.Proxies;
//using CHIS.NotificationCenter.Application.Models.CommonModels;
//using CHIS.Share.AuditTrail.Services;


//namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
//{
//    public class RequestEmployeeSmsMessageNotificationCommandHandler : IRequestHandler<RequestEmployeeSmsMessageNotificationCommand, bool>
//    {
//        private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
//        private readonly IMessageSpecificationRepository _messageSpecificationRepository;
//        private readonly ISmsMonitoringRepository _smsMonitoringRepository;
//        private readonly IMessageDispatcherQueries _messageDispatcherQueries;
//        //private readonly IAccessControlProxy _accessControlProxy;
//        private readonly IUtcService _utcService;
//        private readonly ICallContext _callContext;
//        private readonly ISmsService _smsService;
//        //private readonly ILoggingService _loggingService;


//        public RequestEmployeeSmsMessageNotificationCommandHandler(
//            IMessageDispatchItemRepository messageDispatchItemRepository
//            , IAccessControlProxy accessControlProxy
//            , ISmsMonitoringRepository smsMonitoringRepository
//            , IMessageDispatcherQueries messageDispatcherQueries
//            , IMessageSpecificationRepository  messageSpecificationRepository
//            , ICallContext callContext
//            , IUtcService  utcService
//            , ISmsService smsService
//            , ILoggingService loggingService
//            )
//        {
//            _messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
//            _smsMonitoringRepository = smsMonitoringRepository ?? throw new ArgumentNullException(nameof(smsMonitoringRepository));
//            _messageDispatcherQueries = messageDispatcherQueries ?? throw new ArgumentNullException(nameof(messageDispatcherQueries));
//            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
//            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
//            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
//            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
//            //_accessControlProxy = accessControlProxy ?? throw new ArgumentNullException(nameof(accessControlProxy));
//            //_loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
//        }

//        public async Task<bool> Handle(RequestEmployeeSmsMessageNotificationCommand request, CancellationToken cancellationToken)
//        {
//            if (request == null)
//            {
//                throw new ArgumentNullException(nameof(request));
//            }

//            string tenantId = _callContext.TenantId;
//            string hospitalId = _callContext.HospitalId;

//            foreach (var smsMessage in request.SmsMessages)
//            {
//                DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();

//                DateTime? reservedSmsDateTime = smsMessage.ReservedSmsDateTime == null ? currentLocalDateTime : smsMessage.ReservedSmsDateTime;


//                MessageDispatchItem messageDispatchItem = new MessageDispatchItem(
//                    tenantId: tenantId
//                    , hospitalId: hospitalId
//                    , serviceType: Domain.Enum.NotificationServiceType.SMS
//                    , serviceCode: smsMessage.ServiceCode
//                    , senderId: smsMessage.SenderId
//                    , isUsingPredefinedContent: false
//                    , content: smsMessage.Content
//                    , encounterId: string.Empty
//                    , sentTimeStamp: currentLocalDateTime
//                    , sentTimeStampUtcPack: _utcService.GetUtcPack(currentLocalDateTime)
                   
//                    , isReservedSms: smsMessage.IsReservedSms
//                    , reservedSmsDateTime: reservedSmsDateTime
//                    , reservedSmsDateTimeUtcPack : _utcService.GetUtcPack(reservedSmsDateTime)

//                    , referenceId: string.Empty
//                    //, trace: _loggingService.GetTraceSerializer(Share.AuditTrail.Model.WhatEvent.Create)
//                    );
//                foreach (ContentParameterDto contentParameter in smsMessage.ContentParameters)
//                {
//                    messageDispatchItem.AddContentParameter(new ContentParameter() { ParameterValue = contentParameter.ParameterValue, TenantId = tenantId, HospitalId = hospitalId });
//                }

//                string messageDispatchItemId = _messageDispatchItemRepository.Create(messageDispatchItem).Id;

               


//                List<SmsRecipientDto> smsRecipientDtos = new List<SmsRecipientDto>();

//                SmsRecipientDto smsRecipient = null;


//                foreach (var employee in smsMessage.AssignedEmployeeRecipients)
//                {
//                    if (employee.SmsRecipientType == SmsRecipientType.Employee)
//                    {
//                        smsRecipient = await _messageDispatcherQueries.FindEmployeeSmsRecipient(employee.EmployeeId).ConfigureAwait(false);
//                        smsRecipientDtos.Add(smsRecipient);
//                    }
//                    else if (employee.SmsRecipientType == SmsRecipientType.EmployeeNotRegistered)
//                    {
//                        smsRecipient = new SmsRecipientDto();
//                        smsRecipient.ActorId = "None";
//                        smsRecipient.Name = employee.EmployeeName;
//                        smsRecipient.SmsRecipientType = SmsRecipientType.EmployeeNotRegistered;
//                        smsRecipient.Mobile = employee.Mobile;
//                        smsRecipientDtos.Add(smsRecipient);
//                    }
//                }

//                // DESC : SMS ServiceCode에 정의된 직원 수신자 자동 매핑
//                var messageSpecification = await _messageSpecificationRepository.FindByServiceCode(smsMessage.ServiceCode).ConfigureAwait(false);
//                foreach (var employee in messageSpecification.EmployeeRecipients)
//                {
//                    smsRecipient = await _messageDispatcherQueries.FindEmployeeSmsRecipient(employee.EmployeeId).ConfigureAwait(false);
//                    smsRecipientDtos.Add(smsRecipient);
//                }
              
//                if (smsRecipientDtos.Count > 0)
//                {

//                    if (smsMessage.IsUsingPredefinedContent)
//                    {        
//                        if (smsMessage.ContentParameters.Count > 0)
//                        {
//                            smsMessage.Content = string.Format(messageSpecification.PredefinedContent, smsMessage.ContentParameters.Select(i => i.ParameterValue).ToArray());
//                        }
//                    }

//                    _smsMonitoringRepository.CreateSmsSendLog(new SmsSendLog(
//                          tenantId: _callContext.TenantId
//                         , hospitalId: _callContext.HospitalId
//                           , content: smsMessage.Content
//                            , callingNumber: "" // DESC : 삭제검토
//                           , isReservedSms: smsMessage.IsReservedSms
//                           , reservedTime: reservedSmsDateTime
//                           , executionTime: currentLocalDateTime
//                           , smsProgressStatus: SmsProgressStatus.BeforeProgress
//                           , messageDispatchItemId: messageDispatchItemId
//                           , smsTraceId: string.Empty
//                           , callStatusCode: string.Empty
//                           , errorMessage: string.Empty
//                           , senderId: string.IsNullOrEmpty(smsMessage.SenderId) ? "System" : smsMessage.SenderId
//                           , smsRecipientType: smsRecipient.SmsRecipientType
//                           , serviceCode : smsMessage.ServiceCode
//                           ));

//                    foreach (var entry in smsRecipientDtos)
//                    {
//                        _smsMonitoringRepository.CreateSmsReceiveLog(new SmsReceiveLog(
//                                                  tenantId: _callContext.TenantId
//                                                 , hospitalId: _callContext.HospitalId
//                                                 , smsRecipientType: entry.SmsRecipientType
//                                                 , name: entry.Name == null ? "" : entry.Name
//                                                 , mobile: entry.Mobile == null ? "" : entry.Mobile.Trim().Replace("-", "")
//                                                 , content: smsMessage.Content
//                                                 , isSuccess: false
//                                                 , isAgreeToUsePrivacyData: true
//                                                 //, sentTimeStamp: currentLocalDateTime
//                                                 //, sentTimeStampUtcPack: _utcService.GetUtcPack(currentLocalDateTime)
//                                                 , messageDispatchItemId: messageDispatchItemId
//                                                 , actorId: string.IsNullOrEmpty(entry.ActorId) ? "None" : entry.ActorId
//                                                 , patientContactRelationShipCode: string.Empty
//                                                 , patientContactClassificationCode: string.Empty
//                                                 ));

//                    }
       

//                }


//                await _messageDispatchItemRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);

//                if (!smsMessage.IsReservedSms)
//                {
//                    await _smsService.SendSmsMessage(messageDispatchItem.Id).ConfigureAwait(false);
//                }

//            }

//            return true;
//        }
//    }

//}
