using CHIS.NotificationCenter.Application.Commands.MessageSpecification;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using CHIS.NotificationCenter.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using CHIS.Framework.Core;
using CHIS.Framework.Middleware;
using CHIS.Share.AuditTrail.Services;
using System.Reflection;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.Enums;

namespace CHIS.NotificationCenter.Application.Commands.MessageSpecification
{
    public class RegisterMessageSpecificationCommandHandler : IRequestHandler<RegisterMessageSpecificationCommand, string>
    {
        private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        private readonly ICallContext _callContext;
        private readonly ILoggingService _loggingService;
        private readonly ITimeManager _timeManager;

        // Using DI to inject infrastructure persistence Repositories
        public RegisterMessageSpecificationCommandHandler(
            IMessageSpecificationRepository messageSpecificationRepository
            , ICallContext callContext
            , ITimeManager timeManager
            , ILoggingService loggingService)
        {
            _messageSpecificationRepository = messageSpecificationRepository ?? throw new ArgumentNullException(nameof(messageSpecificationRepository));
            _callContext = callContext ?? throw new ArgumentNullException(nameof(callContext));
            _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
            _timeManager = timeManager ?? throw new ArgumentException(nameof(timeManager));
        }

        public async Task<string> Handle(RegisterMessageSpecificationCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;
            DateTime currentUtcDateTime = _timeManager.GetUTCNow();
            CHIS.Share.AuditTrail.Model.Trace getTrace = _loggingService.GetTrace(Share.AuditTrail.Model.WhatEvent.Create, MethodBase.GetCurrentMethod().DeclaringType.FullName);

            var createTrace = new Trace(new WhoTrace(getTrace.WhoTrace.EmployeeId, getTrace.WhoTrace.EmployeeDisplayId, getTrace.WhoTrace.EmployeeName),
                new WhenTrace(getTrace.WhenTrace.DateTime ?? currentUtcDateTime, getTrace.WhenTrace.TimeZoneId, getTrace.WhenTrace.DateTimeUtc ?? currentUtcDateTime),
                new WhereTrace(getTrace.WhereTrace.IpAddress, getTrace.WhereTrace.ViewId, getTrace.WhereTrace.MethodName),
                new WhatTrace(WhatEvent.Create));


            bool isAddRecipient = request.IsAddRecipient ?? false;
            Domain.AggregateModels.MessageSpecificationAggregate.MessageSpecification messageSpecification = new Domain.AggregateModels.MessageSpecificationAggregate.MessageSpecification
                (
                    tenantId: _callContext.TenantId
                    , hospitalId: _callContext.HospitalId
                    , serviceType: request.ServiceType
                    , serviceCode: request.ServiceCode
                    , messageCategory: request.MessageCategory
                    , classification: request.Classification
                    , description: request.Description
                    , predefinedContent: request.PredefinedContent
                    , postActionType: request.PostActionType
                    , isForceToSendInboxSmsMessage: request.IsForceToSendInboxSmsMessage
                    , isSelectPatientByActiveEncounter: request.IsSelectPatientByActiveEncounter
                    , isSystemProperty: (request.IsSystemProperty == true) ? true : false
                    , isDeleted: request.IsDeleted ?? false
                    , dataFirstRegisteredDateTimeUtc: currentUtcDateTime
                    , dataLastModifiedDateTimeUtc: currentUtcDateTime
                    , isAddRecipient: isAddRecipient
                    , messageCallbackNoConfigId: request.MessageCallbackNoConfigId
                    , trace: createTrace
                );

            if (isAddRecipient)
            {
                addRecipient(request, tenantId, hospitalId, currentUtcDateTime, messageSpecification);
            }
                
            messageSpecification = _messageSpecificationRepository.Create(messageSpecification);

            await _loggingService.PrepareMessageWithAuditTrailAsync(
                messageSpecification, "1.0.0", Share.AuditTrail.Enum.TypeOfActionType.Addition, Share.AuditTrail.Enum.EventCoverageType.Full).ConfigureAwait(false);

            await _messageSpecificationRepository.UnitOfWork.SaveEntitiesWithMessagingAsync().ConfigureAwait(false);
            return messageSpecification.Id;


        }

        /// <summary>
        /// 추가 수신 설정
        /// </summary>
        /// <param name="request"></param>
        /// <param name="tenantId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="currentUtcDateTime"></param>
        /// <param name="messageSpecification"></param>
        private static void addRecipient(RegisterMessageSpecificationCommand request, string tenantId
            , string hospitalId, DateTime currentUtcDateTime
            , Domain.AggregateModels.MessageSpecificationAggregate.MessageSpecification messageSpecification)
        {
            #region add registerRecipientPolicy
            foreach (RegisterDepartmentPolicyDto registerRecipientPolicy in request.DepartmentPolicies)
            {


                try
                {
                    DepartmentPolicy regDepartmentPolicy = new DepartmentPolicy(protocolCode: registerRecipientPolicy.ProtocolCode
                        , departmentId: registerRecipientPolicy.DepartmentId
                        , occupationId: registerRecipientPolicy.OccupationId
                        , jobPositionId: registerRecipientPolicy.JobPositionId
                        , workPlaceId: registerRecipientPolicy.WorkPlaceId
                        , tenantId: tenantId
                        , hospitalId: hospitalId
                        , dataFirstRegisteredDateTimeUtc: currentUtcDateTime
                        , dataLastModifiedDateTimeUtc: currentUtcDateTime);

                    messageSpecification.DepartmentPolicies.Add(regDepartmentPolicy);
                    
                }
                catch (Exception e)
                {

                    throw new Exception(e.ToString());
                }

            }

            #endregion


            #region add registerEncounterPolicy
            foreach (RegisterEncounterPolicyDto registerEncounterPolicy in request.EncounterPolicies)
            {
                messageSpecification.EncounterPolicies.Add(new EncounterPolicy()
                {
                    ProtocolCode = registerEncounterPolicy.ProtocolCode
                    ,
                    TenantId = tenantId
                    ,
                    HospitalId = hospitalId
                    ,
                    DataFirstRegisteredDateTimeUtc = currentUtcDateTime
                    ,
                    DataLastModifiedDateTimeUtc = currentUtcDateTime
                });
            }
            #endregion

            #region add registerEmployRecipient
            foreach (RegisterEmployeeRecipientDto registerEmployRecipient in request.EmployeeRecipients)
            {
                messageSpecification.EmployeeRecipients.Add(new EmployeeRecipient()
                {
                    EmployeeId = registerEmployRecipient.EmployeeId
                    ,
                    TenantId = tenantId
                    ,
                    HospitalId = hospitalId
                    ,
                    DataFirstRegisteredDateTimeUtc = currentUtcDateTime
                    ,
                    DataLastModifiedDateTimeUtc = currentUtcDateTime
                });
            }
            #endregion
        }
    }
}
