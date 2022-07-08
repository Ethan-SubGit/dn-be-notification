using CHIS.Framework.Core.Extension.Messaging;
using CHIS.Framework.Core.Extension.Messaging.EventBus.Abstractions;
using CHIS.Framework.Middleware;
using CHIS.Framework.Core;
using CHIS.Framework.Core.Claims;
using CHIS.Framework.Core.Configuration;
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
using CHIS.NotificationCenter.Application.Proxies;
using CHIS.NotificationCenter.Application.Commands.MessageDispatcher;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.Share.NotificationCenter.Enum;
using CHIS.Share.NotificationCenter.Models;
using CHIS.Share.NotificationCenter.Event;
using CHIS.NotificationCenter.Application.Infrastructure.Util;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.EventHandling
{
    public class PatientSmsMessageSendIntegrationEventHandler : IIntegrationEventHandler<CHIS.Share.NotificationCenter.Event.PatientSmsMessageSendIntegrationEvent>
    {

        private readonly ISmsMonitoringRepository _smsMonitoringRepository;
        private readonly IMediator _mediator;

        public PatientSmsMessageSendIntegrationEventHandler(
            IMessageDispatchItemRepository messageDispatchItemRepository
            , ISmsMonitoringRepository smsMonitoringRepository
            //, IPatientInformationQueries patientInformationQueries
            , IAccessControlProxy accessControlProxy
            , ICallContext callContext
            //, IUtcService utcService
            //, ISmsService smsService
            , IMessagingService messagingService
            , IMediator mediator
            //, CHIS.Share.AuditTrail.Services.ILoggingService loggingService
            ) : base(callContext, messagingService)
        {
            //_messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            _smsMonitoringRepository = smsMonitoringRepository ?? throw new ArgumentNullException(nameof(smsMonitoringRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public override async Task Handle(CHIS.Share.NotificationCenter.Event.PatientSmsMessageSendIntegrationEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentException(nameof(@event));
            }
            await this.HandleSubscriberTaskAsync(@event
                , ConfigurationManager.AppSettings.Domain
                , (_smsMonitoringRepository.UnitOfWork as Object) as DbContext
                , async () =>
                {
                    RequestPatientSmsMessageNotificationCommand requestPatientSmsMessageNotificationCommand =
                    new RequestPatientSmsMessageNotificationCommand();

                    Models.CommonModels.PatientSmsMessageDto smsMessageDto = null;

                    foreach (var smsMessage in @event.PatientSmsMessages)
                    {
                        if (smsMessage == null)
                        {
                            continue;
                        }
                        smsMessageDto = new Models.CommonModels.PatientSmsMessageDto();
                        smsMessageDto.ServiceCode = smsMessage.ServiceCode;
                        smsMessageDto.MessageTemplateId = smsMessage.MessageTemplateId;
                        smsMessageDto.SmsRecipientType = (Domain.Enum.SmsRecipientType)smsMessage.SmsRecipientType;
                        smsMessageDto.SenderId = smsMessage.SenderId;
                        smsMessageDto.PatientId = smsMessage.PatientId;
                        smsMessageDto.Mobile = smsMessage.Mobile;
                        smsMessageDto.IsReservedSms = smsMessage.IsReservedSms;
                        smsMessageDto.ReservedSmsDateTime = smsMessage.ReservedSmsDateTime;
                        smsMessageDto.Content = smsMessage.Content;
                        smsMessageDto.ContactClassificationCode = smsMessage.ContactClassificationCode;
                        smsMessageDto.ContactRelationShipCode = smsMessage.ContactRelationShipCode;
                        smsMessageDto.IsUsingPredefinedContent = smsMessage.IsUsingPredefinedContent;

                        smsMessageDto.ContentParameters = SharedTypeConverter.ConvertContentParameters(smsMessage.ContentParameters);

                        requestPatientSmsMessageNotificationCommand.SmsMessages.Add(smsMessageDto);

                    }

                    //전송할 sms 메시지가 있는 경우에만 호출
                    if (requestPatientSmsMessageNotificationCommand.SmsMessages.Count > 0)
                    {
                        await _mediator.Send(requestPatientSmsMessageNotificationCommand).ConfigureAwait(false);
                    }


                }).ConfigureAwait(false);
            #region 임시주석
            /*
                try
                {   

                    await this.HandleSubscriberTaskAsync(@event, ConfigurationManager.AppSettings.Domain,
                           (_smsMonitoringRepository.UnitOfWork as Object) as DbContext,
                           async () =>
                           {
                               RequestPatientSmsMessageNotificationCommand requestPatientSmsMessageNotificationCommand =
                            new RequestPatientSmsMessageNotificationCommand();

                               Models.CommonModels.PatientSmsMessageDto smsMessageDto = null;

                               foreach (var smsMessage in @event.PatientSmsMessages)
                               {
                                   smsMessageDto = new Models.CommonModels.PatientSmsMessageDto();
                                   smsMessageDto.ServiceCode = smsMessage.ServiceCode;
                                   smsMessageDto.MessageTemplateId = smsMessage.MessageTemplateId;
                                   smsMessageDto.SmsRecipientType = (Domain.Enum.SmsRecipientType)smsMessage.SmsRecipientType;
                                   smsMessageDto.SenderId = smsMessage.SenderId;
                                   smsMessageDto.PatientId = smsMessage.PatientId;
                                   smsMessageDto.Mobile = smsMessage.Mobile;
                                   smsMessageDto.IsReservedSms = smsMessage.IsReservedSms;
                                   smsMessageDto.ReservedSmsDateTime = smsMessage.ReservedSmsDateTime;
                                   smsMessageDto.Content = smsMessage.Content;
                                   smsMessageDto.ContactClassificationCode = smsMessage.ContactClassificationCode;
                                   smsMessageDto.ContactRelationShipCode = smsMessage.ContactRelationShipCode;
                                   smsMessageDto.IsUsingPredefinedContent = smsMessage.IsUsingPredefinedContent;

                                   smsMessageDto.ContentParameters = SharedTypeConverter.ConvertContentParameters(smsMessage.ContentParameters);



                                   requestPatientSmsMessageNotificationCommand.SmsMessages.Add(smsMessageDto);
                           #region 주석(dk)
                           //smsMessageDto = new Models.CommonModels.PatientSmsMessageDto();
                           //smsMessageDto.ServiceCode = smsMessage.ServiceCode;
                           //smsMessageDto.SenderId = smsMessage.SenderId;
                           //employeeSmsMesmsMessageDtossageDto.Content = smsMessage.Content;
                           //smsMessageDto.IsReservedSms = smsMessage.IsReservedSms;
                           //smsMessageDto.ReservedSmsDateTime = smsMessage.ReservedSmsDateTime;

                           //foreach (var recipient in smsMessage.AssignedEmployeeRecipients)
                           //{
                           //    employee = new Models.CommonModels.AssignedEmployeeRecipient();
                           //    employee.EmployeeId = recipient.EmployeeId;
                           //    employee.EmployeeName = recipient.EmployeeName;
                           //    employee.SmsRecipientType = (Domain.Enum.SmsRecipientType)recipient.SmsRecipientType;
                           //    employee.Mobile = recipient.Mobile;
                           //    employeeSmsMessageDto.AssignedEmployeeRecipients.Add(employee);
                           //}


                           //requestEmployeeSmsMessageNotificationCommand.SmsMessages.Add(employeeSmsMessageDto); 
                           #endregion
                       }

                               bool commandResult = await _mediator.Send(requestPatientSmsMessageNotificationCommand);
                       #region 주석
                       //string tenantId = _callContext.TenantId;
                       //string hospitalId = _callContext.HospitalId;

                       //if (@event == null)
                       //{
                       //    throw new ArgumentException(nameof(@event));
                       //}

                       //foreach (var smsMessage in @event.PatientSmsMessages)
                       //{

                       //    DateTime currentTimeLocal = _utcService.GetCurrentLocalTime();
                       //    DateTime? reservedSmsDateTime = smsMessage.ReservedSmsDateTime == null ? currentTimeLocal : smsMessage.ReservedSmsDateTime;


                       //    MessageDispatchItem messageDispatchItem = new MessageDispatchItem(
                       //        tenantId: tenantId
                       //        , hospitalId: hospitalId
                       //        , serviceType: Domain.Enum.NotificationServiceType.SMS
                       //        , serviceCode: smsMessage.ServiceCode
                       //        , senderId: smsMessage.SenderId
                       //        , isUsingPredefinedContent: false
                       //        , content: smsMessage.Content
                       //        , encounterId: string.Empty
                       //        , sentTimeStamp: currentTimeLocal
                       //        , sentTimeStampUtcPack : _utcService.GetUtcPack(currentTimeLocal)
                       //        , isReservedSms: smsMessage.IsReservedSms
                       //        , reservedSmsDateTime: reservedSmsDateTime
                       //        , reservedSmsDateTimeUtcPack : _utcService.GetUtcPack(reservedSmsDateTime)
                       //        , referenceId: string.Empty
                       //        );
                       //    string messageDispatchItemId = _messageDispatchItemRepository.Create(messageDispatchItem).Id;

                       //    var smsRecipient = (await _patientInformationQueries.GetPatientContact(smsMessage.PatientId,
                       //                                (Domain.Enum.SmsRecipientType)smsMessage.SmsRecipientType).ConfigureAwait(false));



                       //    if (smsRecipient != null)
                       //    {

                       //        _smsMonitoringRepository.CreateSmsSendLog(new SmsSendLog(
                       //              tenantId: _callContext.TenantId
                       //             , hospitalId: _callContext.HospitalId
                       //               , content: smsMessage.Content
                       //                , callingNumber: "" // DESC : 삭제검토
                       //               , isReservedSms: smsMessage.IsReservedSms
                       //               , reservedTime: smsMessage.ReservedSmsDateTime //notification.MessageDispatchItem.ReservedSmsDateTime ?? NowDtm
                       //               , executionTime: currentTimeLocal
                       //               , smsProgressStatus: SmsProgressStatus.BeforeProgress
                       //               , messageDispatchItemId: messageDispatchItemId// notification.MessageDispatchItem.Id
                       //               , smsTraceId: string.Empty
                       //               , callStatusCode: string.Empty
                       //               , errorMessage: string.Empty
                       //               , senderId: smsMessage.SenderId
                       //               , smsRecipientType: smsRecipient.SmsRecipientType 
                       //               , serviceCode : smsMessage.ServiceCode
                       //               ));

                       //        // 환자정보 사용가능여부 체크
                       //        _smsMonitoringRepository.CreateSmsReceiveLog(new SmsReceiveLog(
                       //                           tenantId: _callContext.TenantId
                       //                          , hospitalId: _callContext.HospitalId
                       //                          , smsRecipientType: smsRecipient.SmsRecipientType
                       //                          , name: smsRecipient.Name == null ? "" : smsRecipient.Name
                       //                          , mobile: smsRecipient.Mobile == null ? "" : smsRecipient.Mobile.Trim().Replace("-", "")
                       //                          , content: smsMessage.Content
                       //                          , isSuccess: false
                       //                          , isAgreeToUsePrivacyData : _accessControlProxy.GetPatientOfAgreeToUsePrivacyData(smsMessage.PatientId)
                       //                          , sentTimeStamp: currentTimeLocal
                       //                          , sentTimeStampUtcPack : _utcService.GetUtcPack(currentTimeLocal)

                       //                          , messageDispatchItemId: messageDispatchItemId //notification.MessageDispatchItem.Id
                       //                          , actorId: smsRecipient.ActorId
                       //                          , patientContactRelationShipCode : smsRecipient.PatientContactRelationShipCode
                       //                          , patientContactClassificationCode : smsRecipient.PatientContactClassificationCode
                       //                          ));

                       //    }

                       //    await _messageDispatchItemRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);

                       // DESC : MQ로 SMS 배치는 데이타만 만들어 놓고, 스케줄러 돌때 일괄 발송처리함.
                       //if (!smsMessage.IsReservedSms)
                       //{
                       //    await _smsService.SendSmsMessage(messageDispatchItem.Id).ConfigureAwait(false);
                       //} 
                       #endregion
                   }).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    var message = $"Fail to register [PatientSmsMessageSendIntegrationEventHandler] error: {e.ToString()} ";

                    throw new Exception(message);
                }
                */
            #endregion
        }


    }
}
