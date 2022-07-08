using CHIS.Framework.Core;
using CHIS.Framework.Core.Extension.Messaging;
using CHIS.Framework.Core.Extension.Messaging.EventBus.Abstractions;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.IntegrationMessages.Commands;
using CHIS.NotificationCenter.Application.Models.PatientMergeModels;
using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.CommandHandling
{
    public class PatientMergingRetriedIntegrationCommandHandler : IIntegrationCommandHandler<PatientMergingRetriedIntegrationCommand>
    {
        //private readonly IPatientRepository _patientRepository;
        private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;

        private readonly IPatientMergingService _patientMergingService;
        private readonly ITimeManager _timeManager;
        
        

        public PatientMergingRetriedIntegrationCommandHandler(ICallContext callContext,
                    IMessagingService messagingService,
                    IMessageDispatchItemRepository messageDispatchItemRepository,
                    //IPatientRepository patientRepository,
                    IPatientMergingService patientMergingService,
                    ITimeManager timeManager) : base(callContext, messagingService)
        {
            //_patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            _patientMergingService = patientMergingService ?? throw new ArgumentNullException(nameof(patientMergingService));
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
        }

        public override async Task Handle(PatientMergingRetriedIntegrationCommand @event)
        {
            if (@event == null) { throw new ArgumentNullException(nameof(@event)); }
            MergeResultDto result = new MergeResultDto();
            string hospitalId = string.Empty;
            await this.HandleSubscriberTaskAsync(
            @event,
            "notificationcenter",
            (_messageDispatchItemRepository.UnitOfWork as Object) as DbContext,
            async () =>
            {

                var mergingDate = _timeManager.GetNow();
                bool isSuccess = true;
                string errorContent = string.Empty;

                try
                {
                    //await _patientMergingService.PatientMergingToPatientAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                    //     @event.MergingPatient.PatientId, @event.WorkerId).ConfigureAwait(false);

                    //await _patientMergingService.PatientMergingToEncounterAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                    //   @event.MergingPatient.PatientId, @event.WorkerId).ConfigureAwait(false);

                    await _patientMergingService.PatientMergingToMessageDispatchItemAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                         @event.MergingPatient.PatientId, @event.WorkerId).ConfigureAwait(false);

                    await _patientMergingService.PatientMergingToSmsReceiveLogAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                         @event.MergingPatient.PatientId, @event.WorkerId).ConfigureAwait(false);
                       
                    await _messageDispatchItemRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);

                }
                catch (Exception e) 
                {
                    isSuccess = false;
                    errorContent = e.Message;
                }


                var patientMergingRespondedIntegrationCommand = new PatientMergingRespondedIntegrationCommand(@event.PrimaryId, mergingDate,
                        "notificationcenter", isSuccess, result.Merges, errorContent, hospitalId);
                await _messagingService.PrepareMessageAsync(patientMergingRespondedIntegrationCommand, "medicalrecordmerging").ConfigureAwait(false);

                await _messagingService.ProcessPreparedMessageAsync().ConfigureAwait(false);

            }).ConfigureAwait(false);

            // 7. Integration Message 전송
            await _messagingService.SendPreparedMessagesAsync("notificationcenter").ConfigureAwait(false);

        }
    }
}
