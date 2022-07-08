using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    public class MedicalRecordMergingReadModel
    {
        #region property
        public string TargetClassificationId { get; set; }
        public DateTime MergingDate { get; set; }
        public string DomainName { get; set; }
        public bool IsCompleted { get; set; }
        public string ErrorContent { get; set; }

        public string MergeResult { get; set; }
        public List<ReadModelMergeResultDto> MergeResults { get; set; }

        public string TenantId { get; set; }
        public string HospitalId { get; set; }
        public string PatientMergingPrimaryId { get; set; }
        #endregion

        public MedicalRecordMergingReadModel()
        {
            //
        }

        public MedicalRecordMergingReadModel(string targetClassificationId, DateTime mergingDate, string domainName, bool isCompleted, string errorContent
            ,string mergeResult, string tenantId, string hospitalId, string patientMergingPrimaryId)
        {
            TargetClassificationId = targetClassificationId;
            MergingDate = mergingDate;
            DomainName = domainName;
            IsCompleted = isCompleted;
            ErrorContent = errorContent;
            TenantId = tenantId;
            HospitalId = hospitalId;
            PatientMergingPrimaryId = patientMergingPrimaryId;
            MergeResults = JsonConvert.DeserializeObject<List<ReadModelMergeResultDto>>(mergeResult);
        }
    }

    public class ReadModelMergeResultDto
    {
        public string Entity { get; set; }
        public string Id { get; set; }
    }
}
