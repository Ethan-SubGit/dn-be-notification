using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CHIS.NotificationCenter.Application.Models.PatientMergeModels;
using CHIS.NotificationCenter.Application.Models.QueryType;

namespace CHIS.NotificationCenter.Application.Services
{
    public interface IPatientMergingService
    {
        #region 합본 인터페이스
        //Task PatientMergingToPatientAsync(
        //            MergeResultDto mergeResult,
        //                string patientMergingPrimaryId,
        //                string closingPatientId,
        //                string mergingPatientId,
        //                string workerEmployeeId);

        //Task PatientMergingToEncounterAsync(
        //    MergeResultDto mergeResult,
        //        string patientMergingPrimaryId,
        //        string closingPatientId,
        //        string mergingPatientId,
        //        string workerEmployeeId);

        Task PatientMergingToMessageDispatchItemAsync(
            MergeResultDto mergeResult,
                string patientMergingPrimaryId,
                string closingPatientId,
                string mergingPatientId,
                string workerEmployeeId);
        Task PatientMergingToSmsReceiveLogAsync(
            MergeResultDto mergeResult,
                string patientMergingPrimaryId,
                string closingPatientId,
                string mergingPatientId,
                string workerEmployeeId);
        #endregion

        #region 합본 롤백 인터페이스

        //Task RollbackPatientMergingToPatientAsync(
        //            MergeResultDto mergeResult,
        //                string patientMergingPrimaryId,
        //                string closingPatientId,
        //                string mergingPatientId,
        //                string workerEmployeeId,
        //                List<ReadModelMergeResultDto> previousPatientMergingResult);

        //Task RollbackPatientMergingToEncounterAsync(
        //    MergeResultDto mergeResult,
        //        string patientMergingPrimaryId,
        //        string closingPatientId,
        //        string mergingPatientId,
        //        string workerEmployeeId,
        //        List<ReadModelMergeResultDto> previousPatientMergingResult);

        Task RollbackPatientMergingToMessageDispatchItemAsync(
            MergeResultDto mergeResult,
                string patientMergingPrimaryId,
                string closingPatientId,
                string mergingPatientId,
                string workerEmployeeId,
                List<ReadModelMergeResultDto> previousPatientMergingResult);
        Task RollbackPatientMergingToSmsReceiveLogAsync(
            MergeResultDto mergeResult,
                string patientMergingPrimaryId,
                string closingPatientId,
                string mergingPatientId,
                string workerEmployeeId,
                List<ReadModelMergeResultDto> previousPatientMergingResult);
        #endregion
    }
}
