using CHIS.Framework.Core;
using CHIS.Framework.Core.Extension.Messaging;
using CHIS.Framework.Core.Extension.Messaging.EventBus.Abstractions;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.IntegrationMessages.Commands;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events;
using CHIS.NotificationCenter.Application.Models.PatientMergeModels;
using CHIS.NotificationCenter.Application.Queries;
using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.EventHandling
{
    public class PatientMergingRollbackIntegrationEventHandler : IIntegrationEventHandler<PatientMergingRollbackIntegrationEvent>
    {

        //private readonly IPatientRepository _patientRepository;
        private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        private readonly IPatientMergingService _patientMergingService;
        private readonly ITimeManager _timeManager;
        //private readonly IMergingPatientProxy _proxy;

        private readonly IMedicalRecordMergingQueries _medicalRecordMergingQueries;
        //private readonly NotificationCenterContext _notificationCenterContext;
        private const string _dbCatalog = "notificationcenter";
        public PatientMergingRollbackIntegrationEventHandler(ICallContext callContext,
            IMessagingService messagingService,
            IMessageDispatchItemRepository messageDispatchItemRepository,
            //IPatientRepository patientRepository,
            IPatientMergingService patientMergingService,
            //IMergingPatientProxy proxy,
            IMedicalRecordMergingQueries medicalRecordMergingQueries,
            ITimeManager timeManager) : base(callContext, messagingService)
        {
            //_patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            _patientMergingService = patientMergingService ?? throw new ArgumentNullException(nameof(patientMergingService));
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            //_proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _medicalRecordMergingQueries = medicalRecordMergingQueries;
        }

        public override async Task Handle(PatientMergingRollbackIntegrationEvent @event)
        {
            if (@event is null) { throw new ArgumentNullException(nameof(@event)); }
            var now = _timeManager.GetNow();

            //var mergeDomainName = this._mergeService.GetMergeDomainName();
            //var thisDomainName = this._commonService.GetDomainName();

            await this.HandleSubscriberTaskAsync(@event,
                        _dbCatalog,
                        (_messageDispatchItemRepository.UnitOfWork as Object) as DbContext,
                        async () =>
                        {
                            var mergingDate = now;
                            bool isSuccess = true;
                            string errorContent = string.Empty;
                            MergeResultDto result = new MergeResultDto();

                            try
                            {
                                //var previousPatientMergingResult = await _proxy.FindPatientMergingResultAsync(@event.PrimaryId, _dbCatalog).ConfigureAwait(false);
                                var previousPatientMergingResult = await _medicalRecordMergingQueries
                                                                    .FindPatientMergingResultAsync(primaryId: @event.PrimaryId, classificationId: _dbCatalog)
                                                                    .ConfigureAwait(false);
                                

                                #region returnValue
                                /*
                                https://integrateddomainmedicalrecord.c-his.com/medical-record-merging/v0/
                                patient-mergings/result-responses?priamryId=258af7b4-7e56-4af9-
                                acba-8f574ab60d79&targetClassificationId=notificationcenter
                                */
                                /*
                                {
                                  "mergingDate": "2019-11-26T13:41:25.2363093",
                                  "domainName": "notificationcenter",
                                  "isCompleted": true,
                                  "errorContent": null,
                                  "merges": [
                                    {
                                        "entity": "Patient",
                                        "id": "892ce28f-14fa-4b2d-a5e4-15145c66ba67"
                                    }
                                  ]
                                } 
                                */
                                #endregion

                                if (previousPatientMergingResult.IsCompleted && previousPatientMergingResult.MergeResults != null && previousPatientMergingResult.MergeResults.Any())
                                {
                                    //patient
                                    //readmodel로 변경했기때문에 메소드 사용할 필요없음 (2020/02/04)
                                    /*
                                    await _patientMergingService.RollbackPatientMergingToPatientAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                                        @event.MergingPatient.PatientId, @event.WorkerId, previousPatientMergingResult).ConfigureAwait(false);
                                    */

                                    //encounter
                                    //readmodel로 변경했기때문에 메소드 사용할 필요없음 (2020/02/04)
                                    /*
                                    await _patientMergingService.RollbackPatientMergingToEncounterAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                                       @event.MergingPatient.PatientId, @event.WorkerId, previousPatientMergingResult).ConfigureAwait(false);
                                    */

                                    //messageDispatchItem
                                    await _patientMergingService.RollbackPatientMergingToMessageDispatchItemAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                                         @event.MergingPatient.PatientId, @event.WorkerId, previousPatientMergingResult.MergeResults).ConfigureAwait(false);

                                    //sms
                                    await _patientMergingService.RollbackPatientMergingToSmsReceiveLogAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                                         @event.MergingPatient.PatientId, @event.WorkerId, previousPatientMergingResult.MergeResults).ConfigureAwait(false);

                                    // 3. 합본 처리 저장                          
                                    await _messageDispatchItemRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);          
                                }
                            }
                            catch (Exception e)
                            {
                                isSuccess = false;
                                errorContent = e.Message;
                            }

                            var patientMergingRespondedIntegrationCommand = new PatientMergingRespondedIntegrationCommand(
                                @event.PrimaryId,
                                mergingDate,                                      
                                _dbCatalog,                  
                                isSuccess,
                                result.Merges,
                                errorContent,
                                @event.HospitalId
                              );

                            await _messagingService.PrepareMessageAsync(patientMergingRespondedIntegrationCommand, "medicalrecordmerging").ConfigureAwait(false);

                            await _messagingService.ProcessPreparedMessageAsync().ConfigureAwait(false);

                        }).ConfigureAwait(false);

            await _messagingService.SendPreparedMessagesAsync(_dbCatalog).ConfigureAwait(false);
        }
    }
}
