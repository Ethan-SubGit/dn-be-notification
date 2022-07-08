using CHIS.NotificationCenter.Application.Commands.MessageDispatcher;
using CHIS.NotificationCenter.Application.Models;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.EmployeeMessageBoxAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CHIS.Framework.Core;
using CHIS.Framework.Middleware;
using CHIS.Share.NotificationCenter.Models;
using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Application.Models.CommonModels;
namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{
    public class RequestInboxMessageNotificationCommandHandler : IRequestHandler<RequestInboxMessageNotificationCommand, string>
    {
        private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        //private readonly IEmployeeMessageBoxRepository _employeeMessageBoxRepository;
        //private readonly IMessageSpecificationRepository _messageSpecificationRepository;

        //private readonly ITimeManager _timeManager;
        private readonly IUtcService _utcService;
        private readonly ITimeManager _timeManager;
        private readonly ICallContext _callContext;
        //private readonly ISmsService _smsService;
        /*      private readonly CHIS.Share.AuditTrail.Services.ILoggingService _loggingService; */

        // Using DI to inject infrastructure persistence Repositories
        public RequestInboxMessageNotificationCommandHandler(IMessageDispatchItemRepository messageDispatchItemRepository,
            IEmployeeMessageBoxRepository employeeMessageBoxRepository
            //, IMessageSpecificationRepository messageSpecificationRepository
            , ICallContext callContext
            , IUtcService utcService
            , ITimeManager timeManager
            //, ISmsService smsService
            //, CHIS.Share.AuditTrail.Services.ILoggingService loggingService //AuditTrail Logging
            )
        {
            _messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            //_employeeMessageBoxRepository = employeeMessageBoxRepository ?? throw new ArgumentNullException(nameof(employeeMessageBoxRepository));
            //_messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
            _timeManager = timeManager ?? throw new ArgumentException(nameof(timeManager));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            //_smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            //_loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService)); //AuditTrail Logging
        }

        public async Task<string> Handle(RequestInboxMessageNotificationCommand request, CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var serializerSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };


                DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();
                DateTime currentUtcDateTime = _timeManager.GetUTCNow();


                string tenantId = _callContext.TenantId;
                string hospitalId = _callContext.HospitalId;

                MessageDispatchItem messageDispatchItem = new MessageDispatchItem(
                     tenantId: tenantId
                    , hospitalId: hospitalId
                    , serviceType: Domain.Enum.NotificationServiceType.Inbox
                    , serviceCode: request.ServiceCode
                    , senderId: request.SenderId
                    , isUsingPredefinedContent: false
                    , title: request.Title
                    , content: request.Content
                    , smsContentByInbox: request.SmsContentByInbox
                    , patientId: request.PatientId
                    , encounterId: request.EncounterId
                    , integrationType: request.IntegrationType
                    , integrationAddress: JsonConvert.SerializeObject(request.IntegrationAddress)
                    , integrationParameter: JsonConvert.SerializeObject(request.IntegrationParameter, serializerSettings)
                    , sentTimeStamp: currentLocalDateTime
                    , sentTimeStampUtcPack: _utcService.GetUtcPack(currentLocalDateTime)
                    , referenceId: request.ReferenceId
                    , dataFirstRegisteredDateTimeUtc: currentUtcDateTime
                    , dataLastModifiedDateTimeUtc: currentUtcDateTime
                    , trace: null
                    //, trace: _loggingService.GetTraceSerializer(Share.AuditTrail.Model.WhatEvent.Create)
                    );

                #region ## 파라미터
                foreach (var contentParameter in request.ContentParameters)
                {
                    ContentParameter addParameter = new ContentParameter()
                    {
                        ParameterValue = contentParameter.ParameterValue,
                        TenantId = tenantId,
                        HospitalId = hospitalId,
                        DataFirstRegisteredDateTimeUtc = currentUtcDateTime,
                        DataLastModifiedDateTimeUtc = currentUtcDateTime,
                        Trace = null
                    };

                    messageDispatchItem.AddContentParameter(addParameter);
                }
                #endregion

                #region ## 수신정책
                foreach (AssignedEmployeeRecipientDto assignedEmployeeInboxRecipient in request.AssignedEmployeeRecipients)
                {
                    AssignedEmployeeRecipient addEmployeeRecipient = new AssignedEmployeeRecipient()
                    {
                        EmployeeId = assignedEmployeeInboxRecipient.EmployeeId,
                        TenantId = tenantId,
                        HospitalId = hospitalId
                    };
                    messageDispatchItem.AddAssignedEmployeeRecipient(addEmployeeRecipient);
                }

                foreach (AssignedDepartmentPolicyDto departmentPolicy in request.AssignedDepartmentPolicies)
                {
                    AssignedDepartmentPolicy addDepartment = new AssignedDepartmentPolicy()
                    {
                        ProtocolCode = departmentPolicy.ProtocolCode,
                        DepartmentId = departmentPolicy.DepartmentId,
                        OccupationId = departmentPolicy.OccupationId,
                        JobPositionId = departmentPolicy.JobPositionId,
                        TenantId = tenantId,
                        HospitalId = hospitalId
                    };

                    messageDispatchItem.AddAssignedDepartmentPolicy(addDepartment);
                }
                foreach (AssignedEncounterPolicyDto encounterPolicy in request.AssignedEncounterPolicies)
                {
                    AssignedEncounterPolicy addEncounterRow = new AssignedEncounterPolicy()
                    {
                        ProtocolCode = encounterPolicy.ProtocolCode,
                        TenantId = tenantId,
                        HospitalId = hospitalId
                    };
                    messageDispatchItem.AddAssignedEncounterPolicy(addEncounterRow);
                }
                #endregion

                #region 첨부파일
                if (request.MessageAttachments != null)
                {
                    foreach (MessageAttachmentDto messageAttachment in request.MessageAttachments)
                    {
                        MessageAttachment addAttachment = new MessageAttachment()
                        {
                            ContentType = messageAttachment.ContentType,
                            Extension = messageAttachment.Extension,
                            FileKey = messageAttachment.FileKey,
                            OriginalFileName = messageAttachment.OriginalFileName,
                            SavedFileName = messageAttachment.SavedFileName,
                            SavedFilePath = messageAttachment.SavedFilePath,
                            FileSize = messageAttachment.FileSize,
                            Url = messageAttachment.Url,
                            TenantId = tenantId,
                            HospitalId = hospitalId
                        };

                        messageDispatchItem.AddMessageAttachment(addAttachment);
                    }
                }
                #endregion


                string messageDispatchItemId = _messageDispatchItemRepository.Create(messageDispatchItem).Id;

                messageDispatchItem.AddMessageDispatchStartedDomainEvent();

                // TO-DO : UnitOfWork Save를 Domain Event에서 처리. (에러시 원시그널 및 SMS가 발송되면 안되므로....
                try
                {
                    await _messageDispatchItemRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    //throw e;
                    throw new Exception("messageDispatchItem.AddMessageDispatchStartedDomainEvent Error", e);
                }


                #region ### 중복의심
                // InboxMessageDispatchStartedDomainEventHandler 에 전송부분이 구현되어있음
                /*
                var messageSpecification =
                  await _messageSpecificationRepository.FindByServiceCode(messageDispatchItem.ServiceCode).ConfigureAwait(false);

                // TO-DO : SMS 전송은 저장후 별도 트랜잭션으로 발송해야 할 것.
                // 요부분은 없애고 sms전송은 배치로 단일화 하는게 좋을수도 있음.
                
                if (messageSpecification.IsForceToSendInboxSmsMessage)
                {
                    await _smsService.SendSmsMessage(messageDispatchItemId: messageDispatchItem.Id).ConfigureAwait(false);

                }
                */
                #endregion


                // TO-DO : AuditTrail Log Sample Code
                //string aggregateVersion = "0.0.1";
                //await _loggingService.SendWithAuditTrailAsync(messageDispatchItem, aggregateVersion, Share.AuditTrail.Enum.TypeOfActionType.Addition
                //    , Share.AuditTrail.Enum.EventCoverageType.Full).ConfigureAwait(false);

                return messageDispatchItemId;
            }
            else
            {
                throw new ArgumentNullException(nameof(request));
            }
        }

    }
}
