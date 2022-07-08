using CHIS.Framework.Layer;
using CHIS.Framework.Middleware;
using CHIS.NotificationCenter.Application.Models.PatientMergeModels;
using CHIS.NotificationCenter.Application.Models.QueryType;
using CHIS.NotificationCenter.Domain.AggregateModels.MessageDispatcherAggregate;
using CHIS.NotificationCenter.Domain.AggregateModels.SmsMonitoringAggregate;
using CHIS.NotificationCenter.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CHIS.NotificationCenter.Application.Services
{

    public class PatientMergingService : DALBase, IPatientMergingService
    {

        //private readonly ITimeManager _timeManager;
        //private readonly IPatientRepository _patientRepository;
        //private readonly IEncounterRepository _encounterRepository;
        private readonly ISmsMonitoringRepository _smsMonitoringRepository;
        private readonly IMessageDispatchItemRepository _messageDispatchItemRepository;
        //private readonly IMergingPatientProxy _proxy;
        private readonly IUtcService _utcService;

        public PatientMergingService(ICallContext context,
            //IPatientRepository patientRepository,
            //IEncounterRepository encounterRepository,
            IMessageDispatchItemRepository messageDispatchItemRepository,
            ISmsMonitoringRepository smsMonitoringRepository,
            IUtcService utcService
            //,IMergingPatientProxy proxy
            //,ITimeManager timeManager
            ) : base(context)
        {
           
            //_timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
            //_patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            //_encounterRepository = encounterRepository ?? throw new ArgumentNullException(nameof(encounterRepository));
            _messageDispatchItemRepository = messageDispatchItemRepository ?? throw new ArgumentNullException(nameof(messageDispatchItemRepository));
            _smsMonitoringRepository = smsMonitoringRepository ?? throw new ArgumentNullException(nameof(smsMonitoringRepository));
            //_proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
            _utcService = utcService ?? throw new ArgumentNullException(nameof(utcService));
        }


        #region 환자 합본
        #region patientId관련정보 변경
        /// <summary>
        /// 합본 1. patient closing
        /// readmodel로 변경했기때문에 메소드 사용할 필요없음 (2020/02/04)
        /// </summary>
        /// <param name="patientMergingPrimaryId"></param>
        /// <param name="closingPatientId"></param>
        /// <param name="mergingPatientId"></param>
        /// <param name="workerEmployeeId"></param>
        /// <returns></returns>
        //public async Task PatientMergingToPatientAsync(
        //    MergeResultDto mergeResult,
        //        string patientMergingPrimaryId,
        //        string closingPatientId,
        //        string mergingPatientId,
        //        string workerEmployeeId)
        //{
        //    var mergingDate = _utcService.GetCurrentLocalTime();
        //    var utcMergingDate = _utcService.GetUtcPack(mergingDate);
        //    //var _mergingPatientGround = new MergingPatientGround(patientMergingPrimaryId,closingPatientId,mergingPatientId, workerEmployeeId,mergingDate,utcMergingDate);
        //    var patient = await _patientRepository.Find(closingPatientId).ConfigureAwait(false);



        //    if (patient != null)
        //    {

        //        patient.IsClosed = true;
        //        mergeResult.Merges.Add(new MergeDto(patient.GetType().Name, patient.Id));
        //    }

        //}
        #endregion

        #region encounter 
        /// <summary>
        /// 합본 2. encounter merge
        /// readmodel로 변경했기때문에 메소드 사용할 필요없음 (2020/02/04)
        /// </summary>
        /// <param name="patientMergingPrimaryId"></param>
        /// <param name="closingPatientId"></param>
        /// <param name="mergingPatientId"></param>
        /// <param name="workerEmployeeId"></param>
        /// <returns></returns>
        //public async Task PatientMergingToEncounterAsync(
        //    MergeResultDto mergeResult,
        //        string patientMergingPrimaryId,
        //        string closingPatientId,
        //        string mergingPatientId,
        //        string workerEmployeeId)
        //{
        //    var mergingDate = _utcService.GetCurrentLocalTime();
        //    var utcMergingDate = _utcService.GetUtcPack(mergingDate);
        //    var _mergingPatientGround = new MergingPatientGround(patientMergingPrimaryId,
        //                                        closingPatientId,
        //                                        mergingPatientId,
        //                                        workerEmployeeId,
        //                                        mergingDate,
        //                                        utcMergingDate);

        //    var encounterTargets = await _encounterRepository.FindPatientMergingTargets(closingPatientId).ConfigureAwait(false);

        //    foreach (var encounter in encounterTargets)
        //    {
        //        encounter.UpdateMergingPatientGround(_mergingPatientGround);
        //        mergeResult.Merges.Add(new MergeDto(encounter.GetType().Name, encounter.Id));

        //    }


        //}
        #endregion

        #region messageDispatchItemId
        /// <summary>
        /// 합본 3. messageDispatchItem merge
        /// </summary>
        /// <param name="patientMergingPrimaryId"></param>
        /// <param name="closingPatientId"></param>
        /// <param name="mergingPatientId"></param>
        /// <param name="workerEmployeeId"></param>
        /// <returns></returns>
        public async Task PatientMergingToMessageDispatchItemAsync(
            MergeResultDto mergeResult,
                string patientMergingPrimaryId,
                string closingPatientId,
                string mergingPatientId,
                string workerEmployeeId)
        {
            var mergingDate = _utcService.GetCurrentLocalTime();
            var utcMergingDate = _utcService.GetUtcPack(mergingDate);
            var _mergingPatientGround = new MergingPatientGround(patientMergingPrimaryId,
                                                closingPatientId,
                                                mergingPatientId,
                                                workerEmployeeId,
                                                mergingDate,
                                                utcMergingDate);
            var messageDispatchItemTargets = await _messageDispatchItemRepository.FindPatientMergingTargets(closingPatientId).ConfigureAwait(false);




            foreach (var messageDispatchItem in messageDispatchItemTargets)
            {
                var entityName = messageDispatchItem.GetType().Name;
                messageDispatchItem.PatientId = mergingPatientId;
                ////업무연동에 환자정보가 있는경우 replace처리
                if (messageDispatchItem.IntegrationParameter != null && messageDispatchItem.IntegrationParameter.Contains("patientid", StringComparison.CurrentCultureIgnoreCase))
                {
                    messageDispatchItem.IntegrationParameter = messageDispatchItem.IntegrationParameter.Replace(closingPatientId, mergingPatientId, StringComparison.CurrentCultureIgnoreCase);
                }
                messageDispatchItem.UpdateMergingPatientGround(_mergingPatientGround);
                if (mergeResult != null)
                {
                    mergeResult.Merges.Add(new MergeDto(entityName, messageDispatchItem.Id));
                }
                
            }


        }
        #endregion

        #region sms log
        /// <summary>
        /// 합본 5. SmsSendLog merge
        /// </summary>
        /// <param name="patientMergingPrimaryId"></param>
        /// <param name="closingPatientId"></param>
        /// <param name="mergingPatientId"></param>
        /// <param name="workerEmployeeId"></param>
        /// <returns></returns>
        public async Task PatientMergingToSmsReceiveLogAsync(
            MergeResultDto mergeResult,
                string patientMergingPrimaryId,
                string closingPatientId,
                string mergingPatientId,
                string workerEmployeeId)
        {
            var mergingDate = _utcService.GetCurrentLocalTime();
            var utcMergingDate = _utcService.GetUtcPack(mergingDate);
            var _mergingPatientGround = new MergingPatientGround(patientMergingPrimaryId,
                                                closingPatientId,
                                                mergingPatientId,
                                                workerEmployeeId,
                                                mergingDate,
                                                utcMergingDate);

            var smsReceiveLogTargets = await _smsMonitoringRepository.FindPatientMergingTargets(closingPatientId).ConfigureAwait(false);



            foreach (var smsReceiveLog in smsReceiveLogTargets)
            {


                smsReceiveLog.UpdateMergingPatientGround(_mergingPatientGround);
                mergeResult.Merges.Add(new MergeDto(smsReceiveLog.GetType().Name, smsReceiveLog.Id));
            }


        }
        #endregion
        #endregion

        #region 환자 합본 롤백
        /// <summary>
        /// 합본 1. patient closing
        /// readmodel로 변경했기때문에 메소드 사용할 필요없음 (2020/02/04)
        /// </summary>
        /// <param name="patientMergingPrimaryId"></param>
        /// <param name="closingPatientId"></param>
        /// <param name="mergingPatientId"></param>
        /// <param name="workerEmployeeId"></param>
        /// <returns></returns>
        //public async Task RollbackPatientMergingToPatientAsync(
        //    MergeResultDto mergeResult,
        //        string patientMergingPrimaryId,
        //        string closingPatientId,
        //        string mergingPatientId,
        //        string workerEmployeeId,
        //         List<ReadModelMergeResultDto> previousPatientMergingResult)
        //{
        //    var mergingDate = _utcService.GetCurrentLocalTime();
        //    var utcMergingDate = _utcService.GetUtcPack(mergingDate);
        //    var _mergingPatientGround = new MergingPatientGround(patientMergingPrimaryId,
        //                                        closingPatientId,
        //                                        mergingPatientId,
        //                                        workerEmployeeId,
        //                                        mergingDate,
        //                                        utcMergingDate);

        //    var rollbackTargetIds = 
        //        previousPatientMergingResult.Where(p => p.Entity == nameof(Patient)).Select(p => p.Id).ToList();


        //    foreach (var mergedId in rollbackTargetIds)
        //    {


        //        var patient = await _patientRepository.Find(mergedId).ConfigureAwait(false);
        //        patient.IsClosed = false;
        //        mergeResult.Merges.Add(new MergeDto(patient.GetType().Name, patient.Id));
        //    }

            

        //}

        /// <summary>
        /// 합본 2. encounter merge
        /// readmodel로 변경이후 실행필요없음(2020/02/04)
        /// </summary>
        /// <param name="patientMergingPrimaryId"></param>
        /// <param name="closingPatientId"></param>
        /// <param name="mergingPatientId"></param>
        /// <param name="workerEmployeeId"></param>
        /// <returns></returns>
        //public async Task RollbackPatientMergingToEncounterAsync(
        //    MergeResultDto mergeResult,
        //        string patientMergingPrimaryId,
        //        string closingPatientId,
        //        string mergingPatientId,
        //        string workerEmployeeId,
        //        List<ReadModelMergeResultDto> previousPatientMergingResult)
        //{
        //    var mergingDate = _utcService.GetCurrentLocalTime();
        //    var utcMergingDate = _utcService.GetUtcPack(mergingDate);
        //    var _mergingPatientGround = new MergingPatientGround(patientMergingPrimaryId,
        //                                   closingPatientId,
        //                                   mergingPatientId,
        //                                   workerEmployeeId,
        //                                   mergingDate,
        //                                   utcMergingDate);

        //    var rollbackTargetIds =
        //        previousPatientMergingResult.Where(p => p.Entity == nameof(Encounter)).Select(p => p.Id).ToList();
  

        //    foreach (var mergedId in rollbackTargetIds)
        //    {
       

        //        var encounter = await _encounterRepository.RetrieveAsync(mergedId).ConfigureAwait(false);
        //        encounter.UpdateMergingPatientGround(_mergingPatientGround);
        //        mergeResult.Merges.Add(new MergeDto(encounter.GetType().Name, encounter.Id));
        //    }

        //}

        /*
        /// <summary>
        /// 합본 3. messageDispatchItem merge
        /// </summary>
        /// <param name="patientMergingPrimaryId"></param>
        /// <param name="closingPatientId"></param>
        /// <param name="mergingPatientId"></param>
        /// <param name="workerEmployeeId"></param>
        /// <returns></returns>
        public async Task RollbackPatientMergingToMessageDispatchItemAsync(
            MergeResultDto mergeResult,
                string patientMergingPrimaryId,
                string closingPatientId,
                string mergingPatientId,
                string workerEmployeeId,
                ResponsePdo previousPatientMergingResult)
        {
            var mergingDate = _utcService.GetCurrentLocalTime();
            var utcMergingDate = _utcService.GetUtcPack(mergingDate);
            var _mergingPatientGround = new MergingPatientGround(patientMergingPrimaryId,
                                           closingPatientId,
                                           mergingPatientId,
                                           workerEmployeeId,
                                           mergingDate,
                                           utcMergingDate);

            var rollbackTargetIds =
                previousPatientMergingResult.Merges.Where(p => p.Entity == nameof(MessageDispatchItem)).Select(p => p.Id).ToList();
   

            foreach (var mergedId in rollbackTargetIds)
            {
                var messageDispatchItem = await _messageDispatchItemRepository.Retrieve(mergedId).ConfigureAwait(false);
                messageDispatchItem.UpdateMergingPatientGround(_mergingPatientGround);

                mergeResult.Merges.Add(new MergeDto(messageDispatchItem.GetType().Name, messageDispatchItem.Id));
            }

        }
        */

        /// <summary>
        /// 합본 3. messageDispatchItem merge
        /// </summary>
        /// <param name="mergeResult"></param>
        /// <param name="patientMergingPrimaryId"></param>
        /// <param name="closingPatientId"></param>
        /// <param name="mergingPatientId"></param>
        /// <param name="workerEmployeeId"></param>
        /// <param name="previousPatientMergingResult"></param>
        /// <returns></returns>
        public async Task RollbackPatientMergingToMessageDispatchItemAsync(
            MergeResultDto mergeResult,
                string patientMergingPrimaryId,
                string closingPatientId,
                string mergingPatientId,
                string workerEmployeeId,
                List<ReadModelMergeResultDto> previousPatientMergingResult)
        {
            var mergingDate = _utcService.GetCurrentLocalTime();
            var utcMergingDate = _utcService.GetUtcPack(mergingDate);
            var _mergingPatientGround = new MergingPatientGround(patientMergingPrimaryId,
                                           closingPatientId,
                                           mergingPatientId,
                                           workerEmployeeId,
                                           mergingDate,
                                           utcMergingDate);

            var rollbackTargetIds =
                previousPatientMergingResult.Where(p => p.Entity == nameof(MessageDispatchItem)).Select(p => p.Id).ToList();


            foreach (var mergedId in rollbackTargetIds)
            {
                var messageDispatchItem = await _messageDispatchItemRepository.Retrieve(mergedId).ConfigureAwait(false);
                messageDispatchItem.UpdateMergingPatientGround(_mergingPatientGround);

                mergeResult.Merges.Add(new MergeDto(messageDispatchItem.GetType().Name, messageDispatchItem.Id));
            }

        }


        /// <summary>
        /// 합본 5. SmsSendLog merge
        /// </summary>
        /// <param name="patientMergingPrimaryId"></param>
        /// <param name="closingPatientId"></param>
        /// <param name="mergingPatientId"></param>
        /// <param name="workerEmployeeId"></param>
        /// <returns></returns>
        public async Task RollbackPatientMergingToSmsReceiveLogAsync(
            MergeResultDto mergeResult,
                string patientMergingPrimaryId,
                string closingPatientId,
                string mergingPatientId,
                string workerEmployeeId,
                List<ReadModelMergeResultDto> previousPatientMergingResult)
        {
            var mergingDate = _utcService.GetCurrentLocalTime();
            var utcMergingDate = _utcService.GetUtcPack(mergingDate);
            var _mergingPatientGround = new MergingPatientGround(patientMergingPrimaryId,
                                           closingPatientId,
                                           mergingPatientId,
                                           workerEmployeeId,
                                           mergingDate,
                                           utcMergingDate);

            var rollbackTargetIds =
                previousPatientMergingResult.Where(p => p.Entity == nameof(SmsReceiveLog)).Select(p => p.Id).ToList();


            foreach (var mergedId in rollbackTargetIds)
            {
                var smsReceiveLog = await _smsMonitoringRepository.FindSmsReceiveLog(mergedId).ConfigureAwait(false);
                smsReceiveLog.UpdateMergingPatientGround(_mergingPatientGround);
                mergeResult.Merges.Add(new MergeDto(smsReceiveLog.GetType().Name, smsReceiveLog.Id));
            }
 
        }

        #endregion
    }
}
