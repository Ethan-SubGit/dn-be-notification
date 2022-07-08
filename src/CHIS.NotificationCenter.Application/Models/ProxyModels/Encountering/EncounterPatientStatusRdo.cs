using System;
using System.Collections.Generic;
using System.Text;
using CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering.Shared;
namespace CHIS.NotificationCenter.Application.Models.ProxyModels.Encountering
{
    public class EncounterPatientStatusRdo
    {
        public Boolean IsInHospital { get; set; }
        public Boolean IsDischargeOrder { get; set; }
        public Boolean IsDischargeNurseReview { get; set; }
        public Boolean IsMedicalRecordReview { get; set; }
        public BusinessItemRdo ReviewStatus { get; set; }
        public Boolean IsDischargeBillPrint { get; set; }
        public Boolean IsDischarge { get; set; }
        public BusinessItemRdo DischargeProgressStatus { get; set; }

        public EncounterPatientStatusRdo() { }

        public EncounterPatientStatusRdo(Boolean isInHospital, Boolean isDischargeOrder, Boolean isDischargeNurseReview, Boolean isMedicalRecordReview,
            BusinessItemRdo reviewStatus, Boolean isDischargeBillPrint, Boolean isDischarge, BusinessItemRdo dischargeProgressStatus)
        {
            IsInHospital = isInHospital;
            IsDischargeOrder = isDischargeOrder;
            IsDischargeNurseReview = isDischargeNurseReview;
            IsMedicalRecordReview = isMedicalRecordReview;
            ReviewStatus = reviewStatus;
            IsDischargeBillPrint = isDischargeBillPrint;
            IsDischarge = isDischarge;
            DischargeProgressStatus = dischargeProgressStatus;
        }
    }
}
