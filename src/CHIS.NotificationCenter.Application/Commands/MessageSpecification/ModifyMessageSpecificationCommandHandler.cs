using CHIS.NotificationCenter.Application.Commands.MessageSpecification;

using CHIS.NotificationCenter.Domain.AggregateModels.MessageSpecificationAggregate;
using CHIS.NotificationCenter.Domain.Exceptions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CHIS.Framework.Core;
using CHIS.Framework.Middleware;
using CHIS.Share.AuditTrail.Services;
using System.Reflection;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.ValueObjects;
using CHIS.NotificationCenter.Domain.AggregateModels.Shared.Enums;

namespace CHIS.NotificationCenter.Application.Commands.MessageSpecification
{
    public class ModifyMessageSpecificationCommandHandler : IRequestHandler<ModifyMessageSpecificationCommand, bool>
    {
        private readonly IMessageSpecificationRepository _messageSpecificationRepository;
        //private readonly ITimeManager _timeManager;
        private readonly ICallContext _callContext;
        private readonly ILoggingService _loggingService;
        private readonly ITimeManager _timeManager;

        // Using DI to inject infrastructure persistence Repositories
        public ModifyMessageSpecificationCommandHandler(
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

        public async Task<bool> Handle(ModifyMessageSpecificationCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            string tenantId = _callContext.TenantId;
            string hospitalId = _callContext.HospitalId;

            DateTime currentUtcDateTime = _timeManager.GetUTCNow();
            DateTime currentLocalDateTime = _timeManager.GetNow();

            CHIS.Share.AuditTrail.Model.Trace getTrace = _loggingService.GetTrace(Share.AuditTrail.Model.WhatEvent.Create, MethodBase.GetCurrentMethod().DeclaringType.FullName);

            var updateTrace = new Trace(new WhoTrace(getTrace.WhoTrace.EmployeeId, getTrace.WhoTrace.EmployeeDisplayId, getTrace.WhoTrace.EmployeeName),
                new WhenTrace(getTrace.WhenTrace.DateTime ?? currentLocalDateTime, getTrace.WhenTrace.TimeZoneId, getTrace.WhenTrace.DateTimeUtc ?? currentUtcDateTime),
                new WhereTrace(getTrace.WhereTrace.IpAddress, getTrace.WhereTrace.ViewId, getTrace.WhereTrace.MethodName),
                new WhatTrace(WhatEvent.Update));


            bool isAddRecipient = request.IsAddRecipient ?? false;
            Domain.AggregateModels.MessageSpecificationAggregate.MessageSpecification messageSpecification =
                await _messageSpecificationRepository.Retrieve(request.Id).ConfigureAwait(false);

            messageSpecification.ServiceCode = request.ServiceCode;
            messageSpecification.MessageCategory = request.MessageCategory;
            messageSpecification.Classification = request.Classification;
            messageSpecification.Description = request.Description;
            messageSpecification.PredefinedContent = request.PredefinedContent;
            messageSpecification.IsSelectPatientByActiveEncounter = request.IsSelectPatientByActiveEncounter;
            messageSpecification.IsForceToSendInboxSmsMessage = request.IsForceToSendInboxSmsMessage;
            messageSpecification.PostActionType = request.PostActionType;
            messageSpecification.IsSystemProperty = request.IsSystemProperty ?? true;
            messageSpecification.IsDeleted = request.IsDeleted ?? false;
            messageSpecification.IsAddRecipient = isAddRecipient;
            messageSpecification.MessageCallbackNoConfigId = request.MessageCallbackNoConfigId;
            messageSpecification.Trace = updateTrace;
            messageSpecification.DataLastModifiedDateTimeUtc = currentUtcDateTime;

            if (isAddRecipient)
            {
                addRecipient(request, tenantId, hospitalId, currentUtcDateTime, messageSpecification);
            }
                
            _messageSpecificationRepository.Update(messageSpecification);

            await _loggingService.PrepareMessageWithAuditTrailAsync(
               messageSpecification, "1.0.0", Share.AuditTrail.Enum.TypeOfActionType.Modification, Share.AuditTrail.Enum.EventCoverageType.Full).ConfigureAwait(false);

            await _messageSpecificationRepository.UnitOfWork.SaveEntitiesWithMessagingAsync().ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// 추가 수신인 처리
        /// </summary>
        /// <param name="request"></param>
        /// <param name="tenantId"></param>
        /// <param name="hospitalId"></param>
        /// <param name="currentUtcDateTime"></param>
        /// <param name="messageSpecification"></param>
        private static void addRecipient(ModifyMessageSpecificationCommand request, string tenantId
            , string hospitalId, DateTime currentUtcDateTime
            , Domain.AggregateModels.MessageSpecificationAggregate.MessageSpecification messageSpecification)
        {
            #region department policies
            foreach (ModifyDepartmentPolicyDto departmentPolicy in request.DepartmentPolicies)
            {
                var tmpRecipientPolicy = messageSpecification.DepartmentPolicies.FirstOrDefault(x => x.Id == departmentPolicy.Id);

                if (tmpRecipientPolicy == null)
                {
                    messageSpecification.DepartmentPolicies.Add(new DepartmentPolicy()
                    {
                        ProtocolCode = departmentPolicy.ProtocolCode,
                        DepartmentId = departmentPolicy.DepartmentId,
                        OccupationId = departmentPolicy.OccupationId,
                        JobPositionId = departmentPolicy.JobPositionId,
                        WorkPlaceId = departmentPolicy.WorkPlaceId,
                        TenantId = tenantId,
                        HospitalId = hospitalId,
                        DataLastModifiedDateTimeUtc = currentUtcDateTime,
                        DataFirstRegisteredDateTimeUtc = currentUtcDateTime

                    });
                }
                else
                {
                    tmpRecipientPolicy.ProtocolCode = departmentPolicy.ProtocolCode;
                    tmpRecipientPolicy.DepartmentId = departmentPolicy.DepartmentId;
                    tmpRecipientPolicy.OccupationId = departmentPolicy.OccupationId;
                    tmpRecipientPolicy.JobPositionId = departmentPolicy.JobPositionId;
                    tmpRecipientPolicy.WorkPlaceId = departmentPolicy.WorkPlaceId;
                    tmpRecipientPolicy.DataLastModifiedDateTimeUtc = currentUtcDateTime;
                    //tmpRecipientPolicy.TenantId = tenantId;
                    //tmpRecipientPolicy.HospitalId = hospitalId;
                }
            }
            #endregion

            #region encounter policies
            foreach (ModifyEncounterPolicyDto encounterPolicy in request.EncounterPolicies)
            {
                var tmpEncounterPolicy = messageSpecification.EncounterPolicies.FirstOrDefault(x => x.Id == encounterPolicy.Id);

                if (tmpEncounterPolicy == null)
                {
                    messageSpecification.EncounterPolicies.Add(new EncounterPolicy()
                    {
                        ProtocolCode = encounterPolicy.ProtocolCode
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
                else
                {
                    tmpEncounterPolicy.ProtocolCode = encounterPolicy.ProtocolCode;
                    tmpEncounterPolicy.DataLastModifiedDateTimeUtc = currentUtcDateTime;
                }
            }
            #endregion

            #region employee policies
            foreach (ModifyEmployeeRecipientDto employRecipient in request.EmployeeRecipients)
            {
                var tmpEmployeeRecipient = messageSpecification.EmployeeRecipients.FirstOrDefault(x => x.Id == employRecipient.Id);

                if (tmpEmployeeRecipient == null)
                {
                    messageSpecification.EmployeeRecipients.Add(new EmployeeRecipient()
                    {
                        EmployeeId = employRecipient.EmployeeId
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
                else
                {
                    tmpEmployeeRecipient.EmployeeId = employRecipient.EmployeeId;
                    tmpEmployeeRecipient.DataLastModifiedDateTimeUtc = currentUtcDateTime;
                }
            }
            #endregion
        }
    }
}
