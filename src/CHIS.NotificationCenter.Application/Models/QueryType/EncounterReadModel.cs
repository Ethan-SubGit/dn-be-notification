using System;
using System.Collections.Generic;
using System.Text;

namespace CHIS.NotificationCenter.Application.Models.QueryType
{
    public class EncounterReadModel
    {
        #region property
        /// <summary>
        /// EncounterId
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// AccountId
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// 수진상태코드
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// 진료구분(외래/응급/입원)코드
        /// </summary>
        public string ClassCode { get; set; }

        /// <summary>
        /// PatientId
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 지시일시
        /// </summary>
        public DateTime? OrderDatetime { get; set; }

        /// <summary>
        /// 수진진료(입원)일시
        /// </summary>
        public DateTime? VisitStartDate { get; set; }

        /// <summary>
        /// 수진퇴실(원)일
        /// </summary>
        public DateTime? VisitEndDate { get; set; }

        /// <summary>
        /// 수진진료(입원)일시
        /// </summary>
        public DateTime? VisitStartDateTime { get; set; }

        /// <summary>
        /// 수진퇴실(원)일시
        /// </summary>
        public DateTime? VisitEndDateTime { get; set; }

        /// <summary>
        /// 가입원여부
        /// </summary>
        public bool IsActivation { get; set; }

        /// <summary>
        /// 의약분업예외사유
        /// </summary>
        public string PatientTypeCode { get; set; }

        public string ThroughCode { get; set; }
        public string PurposeCode { get; set; }
        public string TreatmentTypeCode { get; set; }
        public string EncounterTypeCode { get; set; }
        public bool IsTreatment { get; set; }

        /// <summary>
        /// 입원등록일시
        /// </summary>
        public DateTime? ActivationDateTime { get; set; }

        /// <summary>
        /// 지시의사Id
        /// </summary>
        public string AdmittingPhysicianId { get; set; }

        /// <summary>
        /// 진료과Id
        /// </summary>
        public string AttendingDepartmentId { get; set; }

        /// <summary>
        /// 진료의Id
        /// </summary>
        public string AttendingPhysicianId { get; set; }

        /// <summary>
        /// 주치의Id
        /// </summary>
        public string PrimaryCarePhysicianId { get; set; }

        /// <summary>
        /// 퇴원지시의사
        /// </summary>
        public string DischargeOrdererId { get; set; }

        /// <summary>
        /// 퇴원지시일시
        /// </summary>
        public DateTime? DischargeOrderDatetime { get; set; }

        /// <summary>
        /// 퇴원예정일
        /// </summary>
        public DateTime? DischargeScheduleDatetime { get; set; }
        public string AdminBedId { get; set; }
        public string AdminFacilityBedId { get; set; }
        public string AdminRoomId { get; set; }
        public string AdminWardId { get; set; }
        public string OccupyingBedId { get; set; }
        public string OccupyingFacilityBedId { get; set; }
        public string OccupyingRoomId { get; set; }
        public string OccupyingWardId { get; set; }
        public string EmergencyMedicalAreaCode { get; set; }
        public bool IsInHospital { get; set; }
        public bool IsDischargeOrder { get; set; }
        public bool IsDischargeNurseReview { get; set; }
        public string DischargeNurseReviewerId { get; set; }
        public bool IsMedicalRecordReview { get; set; }
        public string MedicalRecordReviewerId { get; set; }
        public string ReviewStatusCode { get; set; }
        public string CoderReviewerId { get; set; }
        public bool IsDischargeBillPrint { get; set; }
        public string DischargeBillPrinterId { get; set; }
        public bool IsDischarge { get; set; }
        public string DischargeProgressStatusCode { get; set; }
        public bool IsPaymentCertificate { get; set; }
        public string DischargeCashierId { get; set; }
        public bool IsInterimBillIssue { get; set; }
        public DateTime? InterimBillIssueDatetime { get; set; }
        public string InterimBillIssuerId { get; set; }
        public string OutOnPassStatusCode { get; set; }
        public string TransferStatusCode { get; set; }
        public bool IsDrgOfPatient { get; set; }
        public string DrgNumber { get; set; }
        public DateTime? DrgRegistrationDatetime { get; set; }
        public bool IsDrgAdmissionOrder { get; set; }
        public bool IsDrgDiagnosis { get; set; }
        public bool IsDrgSurgery { get; set; }
        public bool IsDrgTarget { get; set; }
        public bool? IsNamesake { get; set; }
        //public List<EncounterSubstitutionReadModelRdo> EncounterSubstitutions { get; set; }
        //public List<AppointmentProfileReadModelRdo> Appointments { get; set; }
        public string AdmissionEncounterId { get; set; }
        public string OutpatientEncounterId { get; set; }
        public string EncounterRemark { get; set; }
        //public List<CoverageReadModelRdo> Coverages { get; set; }
        public DateTime? WardArrivalDatetime { get; set; } 
        #endregion


        public DateTime? CheckinAvaliableDatetime { get; set; }
        public DateTime? EmergencyRoomArrivalDatetime { get; set; }
        public string AdmissionTypeCode { get; set; }
        public string AdmissionSourceCode { get; set; }
        public string ArrivalPathwayCode { get; set; }
        public string EligibilityCheckCode { get; set; }
        public string HospitalId { get; set; }
        public string TenantId { get; set; }
        public string AccountTasLevelCode { get; set; }
        public string EncounterTasLevelCode { get; set; }
        public string CreationNumber { get; set; }
        public DateTime? ScheduleDatetime { get; set; }
        public bool? IsNoShow { get; set; }
        public string AdmissionPlanId { get; set; }
        public bool IsValidDataRow { get; set; }
    }
}
