using CHIS.NotificationCenter.Application.Commands.MessageDispatcher;
using CHIS.NotificationCenter.Application.Models.CommonModels;
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
using CHIS.Share.AuditTrail.Services;
using System.Collections.Generic;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using System.Reflection;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.Enums;

namespace CHIS.NotificationCenter.Application.Commands.MessageDispatcher
{

    public class RegisterCommunicationNoteNotificationCommandHandler : IRequestHandler<RegisterCommunicationNoteNotificationCommand, string>
    {
        private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        //private readonly IEmployeeMessageBoxRepository _employeeMessageBoxRepository;
        private readonly IMessageSpecificationRepository _messageSpecificationRepository;

        private readonly IUtcService _utcService;
        private readonly ICallContext _callContext;
        private readonly ITimeManager _timeManager;
        private readonly ILoggingService _loggingService;
        //private readonly ISmsService _smsService;
        //private readonly CHIS.Share.AuditTrail.Services.ILoggingService _loggingService; //AuditTrail Logging

        // Using DI to inject infrastructure persistence Repositories
        public RegisterCommunicationNoteNotificationCommandHandler(IMessageDispatchItemRepository messageDispatchItemRepository,
            IEmployeeMessageBoxRepository employeeMessageBoxRepository
            , IMessageSpecificationRepository messageSpecificationRepository
            , ICallContext callContext
            , IUtcService utcService
            , ITimeManager timeManager
            , ISmsService smsService
            , CHIS.Share.AuditTrail.Services.ILoggingService loggingService //AuditTrail Logging
            )
        {
            _messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            //_employeeMessageBoxRepository = employeeMessageBoxRepository ?? throw new ArgumentNullException(nameof(employeeMessageBoxRepository));
            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            _timeManager = timeManager ?? throw new ArgumentException(nameof(timeManager));
            //_smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
        }

        public async Task<string> Handle(RegisterCommunicationNoteNotificationCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            DateTime currentLocalDateTime = _utcService.GetCurrentLocalTime();
            DateTime currentUtcDateTime = _timeManager.GetUTCNow();
            CHIS.Share.AuditTrail.Model.Trace getTrace = _loggingService.GetTrace(Share.AuditTrail.Model.WhatEvent.Create, MethodBase.GetCurrentMethod().DeclaringType.FullName);

            var createTrace = new Trace(new WhoTrace(getTrace.WhoTrace.EmployeeId, getTrace.WhoTrace.EmployeeDisplayId, getTrace.WhoTrace.EmployeeName),
                new WhenTrace(getTrace.WhenTrace.DateTime ?? currentUtcDateTime, getTrace.WhenTrace.TimeZoneId, getTrace.WhenTrace.DateTimeUtc ?? currentUtcDateTime),
                new WhereTrace(getTrace.WhereTrace.IpAddress, getTrace.WhereTrace.ViewId, getTrace.WhereTrace.MethodName),
                new WhatTrace(WhatEvent.Create));


            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            string title = string.Empty;
            //string content = string.Empty;
            string content = request.Content;
            bool isUsingPredefinedContent = request.IsUsingPredefinedContent;


            #region 미리 정의된 내용 또는 파라미터에 따라 보내는 내용 변경.
            //사전정의된 내용을 사용하는 경우
            if (isUsingPredefinedContent)
            {

                var serviceCodeRow = _messageSpecificationRepository.FindByServiceCode(request.ServiceCode);
                content = string.IsNullOrEmpty(serviceCodeRow.PredefinedContent) ? "등록된 Template이 없습니다." : serviceCodeRow.PredefinedContent;
            }
            //파라미터의 내용으로 컨텐츠를 변경


            List<string> paramList = new List<string>();
            //Array paramArr = new Array();
            //string[] paramArr = new string[] { };

            foreach (var contentParameter in request.ContentParameters)
            {

                paramList.Add(contentParameter.ParameterValue);
                //messageDispatchItem.AddContentParameter(new ContentParameter() { ParameterValue = contentParameter.ParameterValue, TenantId = tenantId, HospitalId = hospitalId });
            }
            if (paramList.Count > 0)
            {
                content = string.Format(content, paramList.ToArray());
            }


            //title = content.Trim().Length > 50 ? content.Trim().Substring(0, 50) : content.Trim().Substring(0, content.Trim().Length);

            title = getMessageTitle(content);
            #endregion

            MessageDispatchItem messageDispatchItem = new MessageDispatchItem(
                 tenantId: tenantId
                , hospitalId: hospitalId
                , serviceType: Domain.Enum.NotificationServiceType.CommunicationNote
                , serviceCode: request.ServiceCode
                , senderId: request.SenderId
                , isUsingPredefinedContent: request.IsUsingPredefinedContent
                , title: title
                , content: content
                , smsContentByInbox: null
                , patientId: request.PatientId
                , encounterId: request.EncounterId
                , integrationType: null
                , integrationAddress: null
                , integrationParameter: null
                , sentTimeStamp: currentLocalDateTime
                , sentTimeStampUtcPack: _utcService.GetUtcPack(currentLocalDateTime)
                , referenceId: null
                , dataFirstRegisteredDateTimeUtc: currentUtcDateTime
                , dataLastModifiedDateTimeUtc: currentUtcDateTime
                , trace: createTrace
                //, trace : _loggingService.GetTraceSerializer(Share.AuditTrail.Model.WhatEvent.Create)
                );

            messageDispatchItem.CommunicationNoteMessageDeliveryOption = request.CommunicationNoteMessageDeliveryOption;

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

            #region 수신대상자 및 첨부파일 추가
            //수신직원 추가
            //addReceiverEmployee(request, currentUtcDateTime, tenantId, hospitalId, messageDispatchItem);
            addReceiverEmployee(request, currentUtcDateTime, _callContext, messageDispatchItem);

            //수신부서 추가
            addReceiverDepartment(request, currentUtcDateTime, _callContext, messageDispatchItem);

            //encounterPolicy
            addReceiverByEncounterPolicy(request, currentUtcDateTime, _callContext, messageDispatchItem);

            //첨부파일 추가
            addAttachment(request, currentUtcDateTime, _callContext, messageDispatchItem); 
            #endregion



            string messageDispatchItemId = _messageDispatchItemRepository.Create(messageDispatchItem).Id;

            messageDispatchItem.AddMessageDispatchStartedDomainEvent();

            // TO-DO : UnitOfWork Save를 Domain Event에서 처리. (에러시 원시그널 및 SMS가 발송되면 안되므로....

            await _messageDispatchItemRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);

            //CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate.MessageSpecification
            //var messageSpecification =
            //  await _messageSpecificationRepository.FindByServiceCode(messageDispatchItem.ServiceCode).ConfigureAwait(false);


            // TO-DO : SMS 전송은 저장후 별도 트랜잭션으로 발송해야 할 것.
            //if (messageSpecification.IsForceToSendInboxSmsMessage)
            //{
            //    await _smsService.SendSmsMessage(messageDispatchItemId: messageDispatchItem.Id).ConfigureAwait(false);

            //}


            // TO-DO : AuditTrail Log Sample Code
            //string aggregateVersion = "0.0.1";
            //await _loggingService.SendWithAuditTrailAsync(messageDispatchItem, aggregateVersion, Share.AuditTrail.Enum.TypeOfActionType.Addition
            //    , Share.AuditTrail.Enum.EventCoverageType.Full).ConfigureAwait(false);

            return messageDispatchItemId;

        }

        private void addReceiverEmployee(RegisterCommunicationNoteNotificationCommand request, DateTime utcDT, ICallContext callContext, MessageDispatchItem messageDispatchItem)
        {
            foreach (AssignedEmployeeRecipientDto assignedEmployeeInboxRecipient in request.AssignedEmployeeRecipients)
            {
                messageDispatchItem.AddAssignedEmployeeRecipient(
                    new AssignedEmployeeRecipient()
                    {
                        EmployeeId = assignedEmployeeInboxRecipient.EmployeeId,
                        TenantId = callContext.TenantId,
                        HospitalId = callContext.HospitalId,
                        DataFirstRegisteredDateTimeUtc = utcDT,
                        DataLastModifiedDateTimeUtc = utcDT
                    });
            }
        }

        //첨부파일 추가
        private static void addAttachment(RegisterCommunicationNoteNotificationCommand request, DateTime utcDT, ICallContext callContext, MessageDispatchItem messageDispatchItem)
        {
            if (request.MessageAttachments != null)
            {
                foreach (MessageAttachmentDto messageAttachment in request.MessageAttachments)
                {
                    messageDispatchItem.AddMessageAttachment(new MessageAttachment()
                    {
                        ContentType = messageAttachment.ContentType,
                        Extension = messageAttachment.Extension,
                        FileKey = messageAttachment.FileKey,
                        OriginalFileName = messageAttachment.OriginalFileName,
                        SavedFileName = messageAttachment.SavedFileName,
                        SavedFilePath = messageAttachment.SavedFilePath,
                        FileSize = messageAttachment.FileSize,
                        Url = messageAttachment.Url,
                        TenantId = callContext.TenantId,
                        HospitalId = callContext.HospitalId,
                        DataFirstRegisteredDateTimeUtc = utcDT,
                        DataLastModifiedDateTimeUtc = utcDT
                    });
                }
            }
        }

        /// <summary>
        /// encounterPolicy
        /// </summary>
        /// <param name="request"></param>
        /// <param name="currentUtcDateTime"></param>
        /// <param name="tenantId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="messageDispatchItem"></param>
        private static void addReceiverByEncounterPolicy(RegisterCommunicationNoteNotificationCommand request, DateTime utcDT, ICallContext callContext, MessageDispatchItem messageDispatchItem)
        {
            foreach (AssignedEncounterPolicyDto encounterPolicy in request.AssignedEncounterPolicies)
            {
                messageDispatchItem.AddAssignedEncounterPolicy(new AssignedEncounterPolicy()
                {
                    ProtocolCode = encounterPolicy.ProtocolCode,
                    TenantId = callContext.TenantId,
                    HospitalId = callContext.HospitalId,
                    DataFirstRegisteredDateTimeUtc = utcDT,
                    DataLastModifiedDateTimeUtc = utcDT
                });
            }
        }

        /// <summary>
        /// 수신부서 추가
        /// </summary>
        /// <param name="request"></param>
        /// <param name="currentUtcDateTime"></param>
        /// <param name="tenantId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="messageDispatchItem"></param>
        private static void addReceiverDepartment(RegisterCommunicationNoteNotificationCommand request, DateTime utcDT, ICallContext callContext, MessageDispatchItem messageDispatchItem)
        {
            foreach (AssignedDepartmentPolicyDto departmentPolicy in request.AssignedDepartmentPolicies)
            {
                messageDispatchItem.AddAssignedDepartmentPolicy(new AssignedDepartmentPolicy()
                {
                    ProtocolCode = departmentPolicy.ProtocolCode,
                    DepartmentId = departmentPolicy.DepartmentId,
                    OccupationId = departmentPolicy.OccupationId,
                    JobPositionId = departmentPolicy.JobPositionId,
                    TenantId = callContext.TenantId,
                    HospitalId = callContext.HospitalId,
                    DataFirstRegisteredDateTimeUtc = utcDT,
                    DataLastModifiedDateTimeUtc = utcDT
                });
            }
        }

        /// <summary>
        /// 글자길이에 따른 제목 가져오기
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string getMessageTitle(string content)
        {
            string title;
            if (content.Trim().Length > 50)
            {
                title = content.Trim().Substring(0, 50);
            }
            else
            {
                title = content.Trim().Substring(0, content.Trim().Length);
            }

            return title;
        }
    }
}
