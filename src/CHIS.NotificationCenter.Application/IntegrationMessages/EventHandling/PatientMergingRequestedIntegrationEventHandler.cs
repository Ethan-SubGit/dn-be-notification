using CHIS.Framework.Core;
using CHIS.Framework.Core.Extension.Messaging;
using CHIS.Framework.Core.Extension.Messaging.EventBus.Abstractions;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.IntegrationMessages.Commands;
using CHIS.NotificationCenter.Application.IntegrationMessages.Events;
using CHIS.NotificationCenter.Application.Models.PatientMergeModels;
using CHIS.NotificationCenter.Application.Services;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.IntegrationMessages.EventHandling
{
    /// <summary>
    /// 환자합본 eventHandler
    /// </summary>
    public class PatientMergingRequestedIntegrationEventHandler : IIntegrationEventHandler<PatientMergingRequestedIntegrationEvent>
    {
        //private readonly IPatientRepository _patientRepository;
        private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        private readonly IPatientMergingService _patientMergingService;
        private readonly ITimeManager _timeManager;

        public PatientMergingRequestedIntegrationEventHandler(ICallContext callContext,
            IMessagingService messagingService,
            IMessageDispatchItemRepository messageDispatchItemRepository,
            IPatientMergingService patientMergingService,
            ITimeManager timeManager) : base(callContext, messagingService)
        {
            _messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            _patientMergingService = patientMergingService ?? throw new ArgumentNullException(nameof(patientMergingService));
            _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
        }

        /// <summary>
        /// 합본 요청 처리 메서드
        ///   - 실패에 대한 결과도 PatientMergingRespondedIntegrationCommand로 전송해야 함
        ///   - 실패 시 합본 과정 중 변경된 사항이 적용이 되면 안되므로
        ///   - 아래의 주소의 (Back-end) Reliable Messaging & Transaction Development Guide_1.4 찾모
        ///   - 해당 구현의 참조는 "수신과 동시에 다른 메시지 송신" 내용 참조
        /*
           - http://sps.c-his.com/sites/2/TS/Shared%20Documents/10000.%20%EA%B3%B5%EC%9C%A0/9.%20(Back-end)%20Reliable%20Messaging%20%26%20Transaction%20Development%20Guide
           /(Back-end)%20Reliable%20Messaging%20%26%20Transaction%20Development%20Guide_1.4.pptx
        */
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public override async Task Handle(PatientMergingRequestedIntegrationEvent @event)
        {
            // @event가 null인경우 읍답으로 보내줄 primaryId가 없으므로 로그로 직접 찾울 수 밖에 없음
            if (@event is null) { throw new ArgumentNullException(nameof(@event)); }
            MergeResultDto result = new MergeResultDto();
            //string hospitalId = string.Empty;
            string hospitalId = this.Context.HospitalId;
            await this.HandleSubscriberTaskAsync(
                @event, 
                "notificationcenter",
                (_messageDispatchItemRepository.UnitOfWork as Object) as DbContext,
            async () =>
            {
                // 1.합본 시작


                var mergingDate = _timeManager.GetNow();
                bool isSuccess = true;
                string errorContent = string.Empty;

                try
                {
                    //patientId정보 변경
                    //readmodel로 변경했기때문에 메소드 사용할 필요없음 (2020/02/04)
                    /*
                    await _patientMergingService.PatientMergingToPatientAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                    @event.MergingPatient.PatientId, @event.WorkerId).ConfigureAwait(false);
                    */

                    //encounterId정보 변경
                    //readmodel로 변경했기때문에 메소드 사용할 필요없음 (2020/02/04)
                    /*
                    await _patientMergingService.PatientMergingToEncounterAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                       @event.MergingPatient.PatientId, @event.WorkerId).ConfigureAwait(false);
                    */


                    //inbox, note messageDispatchId정보 변경
                    await _patientMergingService.PatientMergingToMessageDispatchItemAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                         @event.MergingPatient.PatientId, @event.WorkerId).ConfigureAwait(false);

                    ////sms receive에 있는 정보 변경.
                    await _patientMergingService.PatientMergingToSmsReceiveLogAsync(result, @event.PrimaryId, @event.ClosingPatient.PatientId,
                         @event.MergingPatient.PatientId, @event.WorkerId).ConfigureAwait(false);

                    // 3. 합본 처리 저장                          
                    await _messageDispatchItemRepository.UnitOfWork.SaveEntitiesAsync().ConfigureAwait(false);

                }
                catch (Exception e) // 3.1 (Exceiption 발생)저장 실패 시
                {
                    isSuccess = false;
                    errorContent = e.Message;
                    //LogWriter.Error<PatientMergingRequestedIntegrationEventHandler>(@event.ToString());

                }


                var patientMergingRespondedIntegrationCommand = new PatientMergingRespondedIntegrationCommand(@event.PrimaryId, mergingDate,
                        "notificationcenter", isSuccess, result.Merges, errorContent, hospitalId);
                await _messagingService.PrepareMessageAsync(patientMergingRespondedIntegrationCommand, "medicalrecordmerging").ConfigureAwait(false);



                // 6. 보낼 IntegrationMessage 처리
                await _messagingService.ProcessPreparedMessageAsync().ConfigureAwait(false);

            }).ConfigureAwait(false);

            // 7. Integration Message 전송
            await _messagingService.SendPreparedMessagesAsync("notificationcenter").ConfigureAwait(false);
        }
    }
}
